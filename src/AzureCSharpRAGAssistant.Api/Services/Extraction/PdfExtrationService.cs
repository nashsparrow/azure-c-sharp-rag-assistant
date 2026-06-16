using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AzureCSharpRAGAssistant.Api.Services.Storage;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Writer;


namespace AzureCSharpRAGAssistant.Api.Services
{
    public class PdfExtrationService : IPdfExtractionService
    {
        private IFileStorageService FileStorageService { get; set; }

        public PdfExtrationService(IFileStorageService fileStorageService)
        {
            FileStorageService = fileStorageService;
        }

        public async Task<string> ExtractPdfs()
        {
            var documents = await FileStorageService.DownloadAllDocuments("documents");
            var pdfText = new StringBuilder();

            foreach (var document in documents)
            {
                using var doc = PdfDocument.Open(document.Content);

                foreach (var page in doc.GetPages())
                {
                    pdfText.AppendLine(page.Text);
                }
            }

            return pdfText.ToString();
        }
    }
}