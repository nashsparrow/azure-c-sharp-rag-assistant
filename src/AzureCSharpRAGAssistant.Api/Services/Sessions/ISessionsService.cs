using AzureCSharpRAGAssistant.Api.Models;

namespace AzureCSharpRAGAssistant.Api.Services.Sessions
{
    public interface ISessionsService
    {
        Task<Session> AddAsync(Session session, CancellationToken cancellationToken = default);
        Task<Session?> UpdateAsync(Session session, CancellationToken cancellationToken = default);
        Task<Session?> GetByIPAndDateAsync(string iP, DateTime date, CancellationToken cancellationToken = default);
        Task<(bool isValid, string reason)> HandleSessionAndReturnValidAsync(string iP, ActivityType activity, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Session>> GetAllSessionsAsync(CancellationToken cancellationToken = default);
    }

    public enum ActivityType
    {
        upload = 1,
        chat = 2
    }
}