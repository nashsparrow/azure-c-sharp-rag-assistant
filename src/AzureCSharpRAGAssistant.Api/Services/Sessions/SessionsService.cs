using AzureCSharpRAGAssistant.Api.Data;
using AzureCSharpRAGAssistant.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace AzureCSharpRAGAssistant.Api.Services.Sessions
{
    public class SessionsService : ISessionsService
    {
        private readonly IAppDBContext _dbContext;

        public SessionsService(IAppDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Session> AddAsync(Session session, CancellationToken cancellationToken = default)
        {
            session.Id = Guid.NewGuid();
            session.Date = DateTime.UtcNow;
            session.ChatCount = 0;
            session.UploadCount = 0;

            await _dbContext.Sessions.AddAsync(session, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return session;
        }

        public async Task<Session?> GetByIPAndDateAsync(string iP, DateTime date, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Sessions
                .FirstOrDefaultAsync(x => x.IP.Equals(iP) && x.Date.Date == date.Date, cancellationToken);
        }

        public async Task<Session?> UpdateAsync(Session session, CancellationToken cancellationToken = default)
        {
            var existingSession = await _dbContext.Sessions.FirstOrDefaultAsync(x => x.Id == session.Id, cancellationToken);

            if (existingSession is null)
            {
                return null;
            }

            existingSession.ChatCount = session.ChatCount;
            existingSession.UploadCount = session.UploadCount;

            await _dbContext.SaveChangesAsync(cancellationToken);

            return existingSession;
        }

        public async Task<IReadOnlyList<Session>> GetAllSessionsAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Sessions
                .OrderByDescending(x => x.Date)
                .ToListAsync(cancellationToken);
        }


        public async Task<(bool isValid, string reason)> HandleSessionAndReturnValidAsync(string iP, ActivityType activity, CancellationToken cancellationToken = default)
        {
            //check session exist for IP
            var session = await GetByIPAndDateAsync(iP, DateTime.UtcNow);

            if (session == null)
            {
                await AddAsync(new Session { IP = iP });
                return (true, "");
            }

            if (activity == ActivityType.upload)
            {
                if (session.UploadCount >= 2)
                {
                    return (false, "Daily Upload count exceeded for this IP");
                }
                else
                {
                    session.UploadCount++;
                    await UpdateAsync(session);
                    return (true, "");
                }
            }

            if (activity == ActivityType.chat)
            {
                if (session.ChatCount >= 10)
                {
                    return (false, "Daily Chat count exceeded for this IP");
                }
                else
                {
                    session.ChatCount++;
                    await UpdateAsync(session);
                    return (true, "");
                }
            }

            return (false, "Invalid Session Data");
        }
    }
}