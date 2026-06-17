using AzureCSharpRAGAssistant.Api.Contracts.Settings;
using AzureCSharpRAGAssistant.Api.Models;
using AzureCSharpRAGAssistant.Api.Services.Embedding;
using AzureCSharpRAGAssistant.Api.Services.Indexing;
using AzureCSharpRAGAssistant.Api.Services.Storage;
using Microsoft.Extensions.Options;
using System.Text;

namespace AzureCSharpRAGAssistant.Api.Services.Processing
{
    public class DocumentProcessingService : IDocumentProcessingService
    {
        private IPdfExtractionService PdfExtractionService { get; set; }
        private ITextCleanupService TextCleanupService { get; set; }
        private IFileStorageService FileStorageService { get; set; }
        private IChunkingService ChunkingService { get; set; }
        private IEmbeddingService EmbeddingService { get; set; }
        private ISearchIndexService SearchIndexService { get; set; }
        private FolderSettings FolderSettings { get; set; }

        public DocumentProcessingService(IPdfExtractionService pdfExtractionService, ITextCleanupService textCleanupService, IFileStorageService fileStorageService,
         IOptions<FolderSettings> folderSettings, IChunkingService chunkingService, IEmbeddingService embeddingService, ISearchIndexService searchIndexService)
        {
            PdfExtractionService = pdfExtractionService;
            TextCleanupService = textCleanupService;
            FileStorageService = fileStorageService;
            FolderSettings = folderSettings.Value;
            ChunkingService = chunkingService;
            EmbeddingService = embeddingService;
            SearchIndexService = searchIndexService;
        }

        public async Task<List<Chunk>> ProcessAllDocuments()
        {
            // Download Files
            var files = await FileStorageService.DownloadAllDocuments(FolderSettings.DocumentsFolder);
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
            var file = await FileStorageService.DownloadDocument(FolderSettings.DocumentsFolder, fileName);
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
                var pages = PdfExtractionService.ExtractPdfPages(file);
                foreach (var page in pages)
                {
                    int chunkIndex = 0;
                    var cleanedText = TextCleanupService.CleanupText(page.Text);
                    var chunkedArray = ChunkingService.ChunkText(file.FileName, page.PageNumber, cleanedText);

                    if (chunkedArray != null)
                    {
                        foreach (var chunk in chunkedArray)
                        {
                            chunkIndex++;
                            chunk.ContentVector = await EmbeddingService.GenerateEmbeddings(chunk.Content);
                            chunk.FileId = fileId.ToString();
                            chunk.ChunkIndex = chunkIndex;
                            chunks.Add(chunk);
                        }

                        var res = await SearchIndexService.IndexChunksAsync(chunkedArray);
                    }
                }
            }
            return chunks;
        }
    }
}