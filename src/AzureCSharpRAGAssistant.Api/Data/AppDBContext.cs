using System.Text.Json;
using AzureCSharpRAGAssistant.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace AzureCSharpRAGAssistant.Api.Data
{
    public class AppDBContext : DbContext, IAppDBContext
    {

        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options) { }

        public DbSet<DocumentRecord> Documents => Set<DocumentRecord>();

        public DbSet<ChatHistory> ChatHistories => Set<ChatHistory>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DocumentRecord>()
                .HasIndex(x => x.ContentHash)
                .IsUnique();

            modelBuilder.Entity<ChatHistory>()
                .Property(x => x.Chunks)
                .HasColumnType("jsonb")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<Chunk>>(v, (JsonSerializerOptions?)null) ?? new List<Chunk>())
                .Metadata.SetValueComparer(
                    new ValueComparer<List<Chunk>>(
                        (c1, c2) => JsonSerializer.Serialize(c1, (JsonSerializerOptions?)null) == JsonSerializer.Serialize(c2, (JsonSerializerOptions?)null),
                        c => c == null ? 0 : JsonSerializer.Serialize(c, (JsonSerializerOptions?)null).GetHashCode(),
                        c => c == null ? new List<Chunk>() : c.ToList()));
        }
    }
}