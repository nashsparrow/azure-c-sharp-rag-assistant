
using System.Security.Cryptography;
using AzureCSharpRAGAssistant.Api.Contracts;
using AzureCSharpRAGAssistant.Api.Contracts.Settings;
using AzureCSharpRAGAssistant.Api.Exceptions;
using AzureCSharpRAGAssistant.Api.Models;
using AzureCSharpRAGAssistant.Api.Services.Processing;
using AzureCSharpRAGAssistant.Api.Services.Sessions;
using AzureCSharpRAGAssistant.Api.Services.Storage;
using Microsoft.Extensions.Options;

namespace AzureCSharpRAGAssistant.Api.Services.Documents
{
    public class DocumentsUploadService : IDocumentsUploadService
    {
        private readonly ILogger<DocumentsUploadService> _logger;
        private readonly IDocumentRecordsService _documentRecordsService;
        private readonly IDocumentProcessingService _documentProcessingService;
        private readonly IFileStorageService _fileStorageService;
        private readonly FolderSettings _folderSettings;

        public DocumentsUploadService(ILogger<DocumentsUploadService> logger, IFileStorageService fileStorageService,
         IDocumentRecordsService documentRecordsService, IDocumentProcessingService documentProcessingService, IOptions<FolderSettings> folderSettings)
        {
            _logger = logger;
            _fileStorageService = fileStorageService;
            _documentRecordsService = documentRecordsService;
            _documentProcessingService = documentProcessingService;
            _folderSettings = folderSettings.Value;
        }

        public async Task<DocumentRecord> UploadDocument(DocumentUploadRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);
            ArgumentNullException.ThrowIfNull(request.File);

            _logger.LogInformation(
                "Uploading File {FileName} with Indexing: {Indexing}.",
                request.File.FileName,
                request.Indexing);

            using var stream = request.File.OpenReadStream();
            var hash = SHA256.HashData(stream);
            var hashString = Convert.ToHexString(hash).ToLowerInvariant();

            await ValidateDuplicateDocument(hashString);

            DocumentRecord? documentRecord = null;
            var fileUploaded = false;

            try
            {
                await _fileStorageService.UploadDocument(request.File, _folderSettings.DocumentsFolder);
                fileUploaded = true;

                documentRecord = await _documentRecordsService.AddAsync(
                    new DocumentRecord
                    {
                        FileName = request.File.FileName,
                        ContentHash = hashString,
                        Indexed = false,
                        Status = DocumentStatus.Uploaded,
                        JobId = request.JobId
                    });

                if (request.Indexing)
                {
                    byte[] fileContent;
                    await using (var fileStream = request.File.OpenReadStream())
                    await using (var memoryStream = new MemoryStream())
                    {
                        await fileStream.CopyToAsync(memoryStream);
                        fileContent = memoryStream.ToArray();
                    }

                    var blobFile = new BlobFileResult { Content = fileContent, FileName = request.File.FileName };
                    var res = await _documentProcessingService.ProcessDocument(documentRecord.Id, blobFile);

                    if (res.Succeeded)
                    {
                        documentRecord.Indexed = true;
                        await _documentRecordsService.UpdateAsync(documentRecord);
                    }

                }

                return documentRecord;
            }
            catch
            {
                if (fileUploaded && documentRecord is null)
                {
                    await _fileStorageService.DeleteDocument(_folderSettings.DocumentsFolder, request.File.FileName);
                }

                throw;
            }
        }

        private async Task ValidateDuplicateDocument(string contentHash)
        {
            if (await _documentRecordsService.GetByContentHashAsync(contentHash) is not null)
            {
                _logger.LogError("Failed to Upload file, Document Already Exist");
                throw new DuplicateDocumentException("Document Already Exist!");
            }
        }
    }
}