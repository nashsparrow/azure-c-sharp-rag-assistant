using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AzureCSharpRAGAssistant.Api.Contracts;
using AzureCSharpRAGAssistant.Api.Models;
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

        public IEnumerable<PdfPage> ExtractPdfPages(BlobFileResult document)
        {
            using var pdf = PdfDocument.Open(document.Content);
            
            foreach (var page in pdf.GetPages())
            {
                yield return new PdfPage
                {
                    PageNumber = page.Number,
                    Text = page.Text
                };
            }
        }
    }
}