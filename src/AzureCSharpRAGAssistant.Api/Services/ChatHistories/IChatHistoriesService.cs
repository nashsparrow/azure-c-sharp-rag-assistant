using AzureCSharpRAGAssistant.Api.Models;

namespace AzureCSharpRAGAssistant.Api.Services.ChatHistories
{
    public interface IChatHistoriesService
    {
        Task<ChatHistory> AddAsync(ChatHistory chatHistory, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<ChatHistory>> SearchAsync(string? question, string? status, CancellationToken cancellationToken = default);
        Task<ChatHistory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
