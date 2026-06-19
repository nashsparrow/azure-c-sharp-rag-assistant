using AzureCSharpRAGAssistant.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.ApplicationInsights;

namespace AzureCSharpRAGAssistant.Api.Data
{
    public interface IAppDBContext
    {
        DbSet<DocumentRecord> Documents { get; }
        DbSet<ChatHistory> ChatHistories { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}