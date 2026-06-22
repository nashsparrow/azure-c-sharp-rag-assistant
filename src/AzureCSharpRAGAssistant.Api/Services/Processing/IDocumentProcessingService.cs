using AzureCSharpRAGAssistant.Api.Contracts.Results;
using AzureCSharpRAGAssistant.Api.Models;

namespace AzureCSharpRAGAssistant.Api.Services.Processing
{
    public interface IDocumentProcessingService
    {
        Task<DocumentProcessingResult> ProcessAllDocuments();

        Task<DocumentProcessingResult> ProcessDocument(string fileName);
    }
}