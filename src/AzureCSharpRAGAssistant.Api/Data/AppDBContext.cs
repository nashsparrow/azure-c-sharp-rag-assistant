using AzureCSharpRAGAssistant.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace AzureCSharpRAGAssistant.Api.Data
{
    public class AppDBContext : DbContext, IAppDBContext
    {

        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options) { }

        public DbSet<DocumentRecord> Documents => Set<DocumentRecord>();

        public DbSet<ChatHistory> ChatHistories => Set<ChatHistory>();


    }
}