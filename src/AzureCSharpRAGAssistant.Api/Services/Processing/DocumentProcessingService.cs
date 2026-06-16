using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AzureCSharpRAGAssistant.Api.Contracts.Settings;
using AzureCSharpRAGAssistant.Api.Services.Storage;

namespace AzureCSharpRAGAssistant.Api.Services.Processing
{
    public class DocumentProcessingService : IDocumentProcessingService
    {
        private IPdfExtractionService PdfExtractionService { get; set; }
        private ITextCleanupService TextCleanupService { get; set; }
        private IFileStorageService FileStorageService { get; set; }

        private FolderSettings FolderSettings { get; set; }

        public DocumentProcessingService(IPdfExtractionService pdfExtractionService, ITextCleanupService textCleanupService, IFileStorageService fileStorageService,
         FolderSettings folderSettings)
        {
            PdfExtractionService = pdfExtractionService;
            TextCleanupService = textCleanupService;
            FileStorageService = fileStorageService;
            FolderSettings = folderSettings;
        }

        public async Task<string> ProcessDocuments()
        {
            // Download Files
            var files = await FileStorageService.DownloadAllDocuments(FolderSettings.DocumentsFolder);

            foreach (var file in files)
            {
                var text = new StringBuilder();
                var extension = Path.GetExtension(file.FileName);

                if (string.Equals(extension, ".pdf", StringComparison.OrdinalIgnoreCase))
                {
                    var doc = PdfExtractionService.ExtractPdf(file);
                    foreach (var page in doc.GetPages())
                    {
                        var cleanedText = TextCleanupService.CleanupText(page.Text);
                        text.Append(page.Text);
                    }

                }
            }
            

        }
    }
}