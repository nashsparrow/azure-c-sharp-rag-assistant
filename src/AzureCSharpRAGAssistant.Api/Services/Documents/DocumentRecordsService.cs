using AzureCSharpRAGAssistant.Api.Data;
using AzureCSharpRAGAssistant.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace AzureCSharpRAGAssistant.Api.Services.Documents
{
    public class DocumentRecordsService : IDocumentRecordsService
    {
        private readonly IAppDBContext _dbContext;
        public DocumentRecordsService(IAppDBContext appDBContext)
        {
            _dbContext = appDBContext;
        }

        public async Task<DocumentRecord> AddAsync(DocumentRecord document, CancellationToken cancellationToken = default)
        {
            document.Id = Guid.NewGuid();
            document.CreatedUtc = DateTime.UtcNow;

            await _dbContext.Documents.AddAsync(document, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return document;
        }

        public async Task<DocumentRecord?> UpdateAsync(DocumentRecord document, CancellationToken cancellationToken = default)
        {
            var existingDocument = await _dbContext.Documents
                .FirstOrDefaultAsync(x => x.Id == document.Id, cancellationToken);

            if (existingDocument is null)
            {
                return null;
            }

            existingDocument.FileName = document.FileName;
            existingDocument.ContentHash = document.ContentHash;
            existingDocument.Indexed = document.Indexed;

            await _dbContext.SaveChangesAsync(cancellationToken);

            return existingDocument;
        }

        public async Task<DocumentRecord?> UpdateStatusByIdAsync(Guid id, DocumentStatus status, CancellationToken cancellationToken = default)
        {
            var existingDocument = await _dbContext.Documents
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (existingDocument is null)
            {
                return null;
            }
            existingDocument.Status = status;

            await _dbContext.SaveChangesAsync(cancellationToken);

            return existingDocument;
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var document = await _dbContext.Documents
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (document is null)
            {
                return false;
            }

            _dbContext.Documents.Remove(document);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<DocumentRecord?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Documents
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyList<DocumentRecord>> GetAllDocumentsAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Documents
                .OrderByDescending(x => x.CreatedUtc)
                .ToListAsync(cancellationToken);
        }

        public async Task<DocumentRecord?> GetByContentHashAsync(string contentHash, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Documents
                .FirstOrDefaultAsync(x => x.ContentHash.Equals(contentHash), cancellationToken);
        }

        public async Task<IReadOnlyList<DocumentRecord>> SearchAsync(string? fileName, CancellationToken cancellationToken = default)
        {
            IQueryable<DocumentRecord> query = _dbContext.Documents;

            if (!string.IsNullOrWhiteSpace(fileName))
            {
                query = query.Where(x => x.FileName.Contains(fileName));
            }

            return await query
                .OrderByDescending(x => x.CreatedUtc)
                .ToListAsync(cancellationToken);
        }
    }
}
