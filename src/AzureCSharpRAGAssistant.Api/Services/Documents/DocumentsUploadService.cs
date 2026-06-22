
using System.Security.Cryptography;
using AzureCSharpRAGAssistant.Api.Contracts;
using AzureCSharpRAGAssistant.Api.Models;
using AzureCSharpRAGAssistant.Api.Services.Processing;
using AzureCSharpRAGAssistant.Api.Services.Storage;

namespace AzureCSharpRAGAssistant.Api.Services.Documents
{
    public class DocumentsUploadService : IDocumentsUploadService
    {
        private readonly ILogger<DocumentsUploadService> _logger;
        private readonly IDocumentRecordsService _documentRecordsService;
        private readonly IDocumentProcessingService _documentProcessingService;
        private readonly IFileStorageService _fileStorageService;

        public DocumentsUploadService(ILogger<DocumentsUploadService> logger, IFileStorageService fileStorageService,
         IDocumentRecordsService documentRecordsService, IDocumentProcessingService documentProcessingService)
        {
            _logger = logger;
            _fileStorageService = fileStorageService;
            _documentRecordsService = documentRecordsService;
            _documentProcessingService = documentProcessingService;
        }

        public async Task<DocumentRecord> UploadDocument(DocumentUploadRequest request)
        {
            _logger.LogInformation("Uploading File {FileName} with Indexing: {Indexing}.", request.File.FileName, request.Indexing);
            using var stream = request.File.OpenReadStream();
            var hash = SHA256.HashData(stream);
            var hashString = Convert.ToHexString(hash).ToLowerInvariant();

            await ValidateDuplicateDocument(hashString);

            await _fileStorageService.UploadDocument(request.File);

            var documentRecord = await _documentRecordsService.AddAsync(
                new DocumentRecord
                {
                    FileName = request.File.FileName,
                    ContentHash = hashString
                });

            if (request.Indexing)
            {
                var res = await _documentProcessingService.ProcessDocument(request.File.FileName);

                if (res.ToList().Count > 0)
                {
                    documentRecord.Indexed = true;
                    await _documentRecordsService.UpdateAsync(documentRecord);
                }
            }

            return documentRecord;
        }

        public async Task ValidateDuplicateDocument(string contentHash)
        {
            if (await _documentRecordsService.GetByContentHashAsync(contentHash) is not null)
            {
                _logger.LogError("Failed to Upload file, Document Already Exist");
                throw new InvalidOperationException("Document Already Exist!");
            }
        }
    }
}