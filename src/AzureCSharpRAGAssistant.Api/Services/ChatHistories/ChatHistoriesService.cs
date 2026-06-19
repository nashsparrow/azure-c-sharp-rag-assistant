using AzureCSharpRAGAssistant.Api.Data;
using AzureCSharpRAGAssistant.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace AzureCSharpRAGAssistant.Api.Services.ChatHistories
{
    public class ChatHistoriesService : IChatHistoriesService
    {
        private readonly IAppDBContext _dbContext;

        public ChatHistoriesService(IAppDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ChatHistory> AddAsync(ChatHistory chatHistory, CancellationToken cancellationToken = default)
        {
            if (chatHistory.Id == Guid.Empty)
            {
                chatHistory.Id = Guid.NewGuid();
            }

            if (chatHistory.CreatedTime == default)
            {
                chatHistory.CreatedTime = DateTime.UtcNow;
            }

            await _dbContext.ChatHistories.AddAsync(chatHistory, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return chatHistory;
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var chatHistory = await _dbContext.ChatHistories
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (chatHistory is null)
            {
                return false;
            }

            _dbContext.ChatHistories.Remove(chatHistory);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<ChatHistory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ChatHistories
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyList<ChatHistory>> SearchAsync(string? question, string? status, CancellationToken cancellationToken = default)
        {
            IQueryable<ChatHistory> query = _dbContext.ChatHistories;

            if (!string.IsNullOrWhiteSpace(question))
            {
                query = query.Where(x => x.Question.Contains(question));
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(x => x.Status == status);
            }

            return await query
                .OrderByDescending(x => x.CreatedTime)
                .ToListAsync(cancellationToken);
        }
    }
}
