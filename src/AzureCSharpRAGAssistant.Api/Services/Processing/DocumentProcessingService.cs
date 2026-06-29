using System.Security.Cryptography;
using AzureCSharpRAGAssistant.Api.Contracts;
using AzureCSharpRAGAssistant.Api.Contracts.Results;
using AzureCSharpRAGAssistant.Api.Contracts.Settings;
using AzureCSharpRAGAssistant.Api.Models;
using AzureCSharpRAGAssistant.Api.Services.Documents;
using AzureCSharpRAGAssistant.Api.Services.Embedding;
using AzureCSharpRAGAssistant.Api.Services.Indexing;
using AzureCSharpRAGAssistant.Api.Services.Storage;
using Microsoft.Extensions.Options;

namespace AzureCSharpRAGAssistant.Api.Services.Processing
{
    public class DocumentProcessingService : IDocumentProcessingService
    {
        private readonly IPdfExtractionService _pdfExtractionService;
        private readonly ITextCleanupService _textCleanupService;
        private readonly IFileStorageService _fileStorageService;
        private readonly IChunkingService _chunkingService;
        private readonly IEmbeddingService _embeddingService;
        private readonly ISearchIndexService _searchIndexService;
        private readonly FolderSettings _folderSettings;
        private readonly IDocumentRecordsService _documentRecordService;

        public DocumentProcessingService(IPdfExtractionService pdfExtractionService, ITextCleanupService textCleanupService, IFileStorageService fileStorageService,
         IOptions<FolderSettings> folderSettings, IChunkingService chunkingService, IEmbeddingService embeddingService, ISearchIndexService searchIndexService,
         IDocumentRecordsService documentRecordService)
        {
            _pdfExtractionService = pdfExtractionService;
            _textCleanupService = textCleanupService;
            _fileStorageService = fileStorageService;
            _folderSettings = folderSettings.Value;
            _chunkingService = chunkingService;
            _embeddingService = embeddingService;
            _searchIndexService = searchIndexService;
            _documentRecordService = documentRecordService;
        }

        public async Task<DocumentProcessingResult> ProcessAllDocuments()
        {
            // Download Files
            var files = await _fileStorageService.DownloadAllDocuments(_folderSettings.DocumentsFolder);
            var chunks = new List<Chunk>();
            foreach (var file in files)
            {
                var chunkArray = await Process(file, file.FileName);
                chunks.AddRange(chunkArray);
            }
            return new DocumentProcessingResult { Succeeded = true, Chunks = chunks };
        }

        public async Task<DocumentProcessingResult> ProcessDocument(Guid documentId, BlobFileResult file)
        {
            var chunks = await Process(file, file.FileName, documentId);
            return new DocumentProcessingResult { Succeeded = true, Chunks = chunks };

        }

        public async Task<List<Chunk>> Process(BlobFileResult file, string fileName, Guid? documentId = null)
        {
            try
            {
                var chunks = new List<Chunk>();
                var fileId = CreateStableDocumentId(file.Content);
                var extension = Path.GetExtension(fileName);
                bool firstChunk = false;
                bool firstClean = false;

                if (string.Equals(extension, ".pdf", StringComparison.OrdinalIgnoreCase))
                {
                    var pages = _pdfExtractionService.ExtractPdfPages(file);
                    await UpdateDocumentStatus(documentId, DocumentStatus.Extracted);

                    foreach (var page in pages)
                    {
                        int chunkIndex = 0;
                        var cleanedText = _textCleanupService.CleanupText(page.Text);
                        if (!firstClean)
                        {
                            await UpdateDocumentStatus(documentId, DocumentStatus.Cleaned);
                            firstClean = true;
                        }

                        if (cleanedText != null)
                        {
                            var chunkedArray = _chunkingService.ChunkText(file.FileName, page.PageNumber, cleanedText);
                            if (!firstChunk)
                            {
                                await UpdateDocumentStatus(documentId, DocumentStatus.Chunked);
                                firstChunk = true;
                            }

                            if (chunkedArray != null)
                            {
                                foreach (var chunk in chunkedArray)
                                {
                                    chunkIndex++;
                                    chunk.ContentVector = await _embeddingService.GenerateEmbeddings(chunk.Content);
                                    chunk.FileId = fileId.ToString();
                                    chunk.ChunkIndex = chunkIndex;
                                    chunks.Add(chunk);
                                }
                                var res = await _searchIndexService.IndexChunksAsync(chunkedArray);
                            }
                        }
                    }
                }
                await UpdateDocumentStatus(documentId, DocumentStatus.Embedded);
                await UpdateDocumentStatus(documentId, DocumentStatus.Indexed);
                return chunks;
            }
            catch (Exception ex)
            {
                await UpdateDocumentStatus(documentId, DocumentStatus.Failed);
                throw;
            }
        }

        private static string CreateStableDocumentId(byte[] content)
        {
            var hash = SHA256.HashData(content);
            return Convert.ToHexString(hash).ToLowerInvariant();
        }


        private async Task UpdateDocumentStatus(Guid? docId, DocumentStatus status)
        {
            if (docId is null)
            {
                return;
            }

            await _documentRecordService.UpdateStatusByIdAsync(docId.Value, status);
        }
    }
}