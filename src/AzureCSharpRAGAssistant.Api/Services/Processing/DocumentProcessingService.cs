using AzureCSharpRAGAssistant.Api.Contracts.Settings;
using AzureCSharpRAGAssistant.Api.Models;
using AzureCSharpRAGAssistant.Api.Services.Embedding;
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
        private FolderSettings FolderSettings { get; set; }

        public DocumentProcessingService(IPdfExtractionService pdfExtractionService, ITextCleanupService textCleanupService, IFileStorageService fileStorageService,
         IOptions<FolderSettings> folderSettings, IChunkingService chunkingService, IEmbeddingService embeddingService)
        {
            PdfExtractionService = pdfExtractionService;
            TextCleanupService = textCleanupService;
            FileStorageService = fileStorageService;
            FolderSettings = folderSettings.Value;
            ChunkingService = chunkingService;
            EmbeddingService = embeddingService;
        }

        public async Task<List<Chunk>> ProcessDocuments()
        {
            // Download Files
            var files = await FileStorageService.DownloadAllDocuments(FolderSettings.DocumentsFolder);
            var chunks = new List<Chunk>();
            foreach (var file in files)
            {
                var fileId = Guid.NewGuid();
                var text = new StringBuilder();
                var extension = Path.GetExtension(file.FileName);
                int chunkIndex = 0;

                if (string.Equals(extension, ".pdf", StringComparison.OrdinalIgnoreCase))
                {
                    var pages = PdfExtractionService.ExtractPdfPages(file);
                    foreach (var page in pages)
                    {
                        var cleanedText = TextCleanupService.CleanupText(page.Text);
                        var chunkedArray = ChunkingService.ChunkText(file.FileName, page.PageNumber, cleanedText);

                        if (chunkedArray != null)
                        {
                            foreach (var chunk in chunkedArray)
                            {
                                chunkIndex++;
                                chunk.Embeddings = await EmbeddingService.GenerateEmbeddings(chunk.Text);
                                chunk.FileId = fileId;
                                chunk.ChunkIndex = chunkIndex;
                                chunks.Add(chunk);
                            }
                        }
                    }
                }
            }
            return chunks;
        }
    }
}