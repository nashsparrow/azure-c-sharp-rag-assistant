using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AzureCSharpRAGAssistant.Api.Contracts;
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

        public PdfDocument ExtractPdf(BlobFileResult document)
        {
            using var doc = PdfDocument.Open(document.Content);
            return doc;
        }
    }
}