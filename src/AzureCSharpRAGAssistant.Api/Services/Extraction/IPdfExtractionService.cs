using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureCSharpRAGAssistant.Api.Contracts;
using UglyToad.PdfPig;

namespace AzureCSharpRAGAssistant.Api.Services
{
    public interface IPdfExtractionService
    {
        PdfDocument ExtractPdf(BlobFileResult document);
    }
}