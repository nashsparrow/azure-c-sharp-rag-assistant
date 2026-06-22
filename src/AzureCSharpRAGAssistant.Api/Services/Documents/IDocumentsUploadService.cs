using AzureCSharpRAGAssistant.Api.Contracts;
using AzureCSharpRAGAssistant.Api.Models;

namespace AzureCSharpRAGAssistant.Api.Services.Documents
{
    public interface IDocumentsUploadService
    {
        Task<DocumentRecord> UploadDocument(DocumentUploadRequest request);
    }
}