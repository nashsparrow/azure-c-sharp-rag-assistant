using AzureCSharpRAGAssistant.Api.Models;

namespace AzureCSharpRAGAssistant.Api.Services.Documents
{
    public interface IDocumentRecordsService
    {
        Task<DocumentRecord> AddAsync(DocumentRecord document, CancellationToken cancellationToken = default);
        Task<DocumentRecord?> UpdateAsync(DocumentRecord document, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<DocumentRecord>> SearchAsync(string? fileName, CancellationToken cancellationToken = default);
        Task<DocumentRecord?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<DocumentRecord?> GetByJobIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<DocumentRecord?> GetByContentHashAsync(string contentHash, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<DocumentRecord>> GetAllDocumentsAsync(CancellationToken cancellationToken = default);
        Task<DocumentRecord?> UpdateStatusByIdAsync(Guid id, DocumentStatus status, CancellationToken cancellationToken = default);
    }
}
