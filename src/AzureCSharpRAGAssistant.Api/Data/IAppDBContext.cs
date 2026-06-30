using AzureCSharpRAGAssistant.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace AzureCSharpRAGAssistant.Api.Data
{
    public interface IAppDBContext
    {
        DbSet<DocumentRecord> Documents { get; }
        DbSet<ChatHistory> ChatHistories { get; }
        DbSet<Session> Sessions { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}