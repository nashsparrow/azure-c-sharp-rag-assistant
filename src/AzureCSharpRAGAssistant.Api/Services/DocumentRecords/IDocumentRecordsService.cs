using AzureCSharpRAGAssistant.Api.Models;

namespace AzureCSharpRAGAssistant.Api.Services.DocumentRecords
{
    public interface IDocumentRecordsService
    {
        Task<DocumentRecord> AddAsync(DocumentRecord document, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<DocumentRecord>> SearchAsync(string? fileName, CancellationToken cancellationToken = default);
        Task<DocumentRecord?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}