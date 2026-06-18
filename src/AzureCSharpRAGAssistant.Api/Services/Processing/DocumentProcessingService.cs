using AzureCSharpRAGAssistant.Api.Contracts.Settings;
using AzureCSharpRAGAssistant.Api.Models;
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

        public DocumentProcessingService(IPdfExtractionService pdfExtractionService, ITextCleanupService textCleanupService, IFileStorageService fileStorageService,
         IOptions<FolderSettings> folderSettings, IChunkingService chunkingService, IEmbeddingService embeddingService, ISearchIndexService searchIndexService)
        {
            _pdfExtractionService = pdfExtractionService;
            _textCleanupService = textCleanupService;
            _fileStorageService = fileStorageService;
            _folderSettings = folderSettings.Value;
            _chunkingService = chunkingService;
            _embeddingService = embeddingService;
            _searchIndexService = searchIndexService;
        }

        public async Task<List<Chunk>> ProcessAllDocuments()
        {
            // Download Files
            var files = await _fileStorageService.DownloadAllDocuments(_folderSettings.DocumentsFolder);
            var chunks = new List<Chunk>();
            foreach (var file in files)
            {
                var chunkArray = await Process(file, file.FileName);
                chunks.AddRange(chunkArray);
            }
            return chunks;
        }

        public async Task<List<Chunk>> ProcessDocument(string fileName)
        {
            var file = await _fileStorageService.DownloadDocument(_folderSettings.DocumentsFolder, fileName);
            var chunks = await Process(file, fileName);
            return chunks;
            
        }

        public async Task<List<Chunk>> Process(Contracts.BlobFileResult file, string fileName)
        {
            var chunks = new List<Chunk>();
            var fileId = Guid.NewGuid();
            var extension = Path.GetExtension(fileName);
            

            if (string.Equals(extension, ".pdf", StringComparison.OrdinalIgnoreCase))
            {
                var pages = _pdfExtractionService.ExtractPdfPages(file);
                foreach (var page in pages)
                {
                    int chunkIndex = 0;
                    var cleanedText = _textCleanupService.CleanupText(page.Text);
                    var chunkedArray = _chunkingService.ChunkText(file.FileName, page.PageNumber, cleanedText);

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
            return chunks;
        }
    }
}