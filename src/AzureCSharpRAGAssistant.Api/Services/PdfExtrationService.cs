using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Writer;


namespace AzureCSharpRAGAssistant.Api.Services
{
    public class PdfExtrationService : IPdfExtractionService
    {
        private IDocumentDownloadService DocumentDownloadService { get; set; }

        public PdfExtrationService(IDocumentDownloadService documentDownloadService)
        {
            DocumentDownloadService = documentDownloadService;
        }

        public async Task<string> ExtractPdfs()
        {
            var documents = await DocumentDownloadService.DownloadAllDocuments("documents");
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