using AzureCSharpRAGAssistant.Api.Contracts;
using AzureCSharpRAGAssistant.Api.Models;

namespace AzureCSharpRAGAssistant.Api.Services
{
    public interface IPdfExtractionService
    {
        IEnumerable<PdfPage> ExtractPdfPages(BlobFileResult document);
    }
}