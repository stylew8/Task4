using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Server.DAL.Interfaces;
using Server.DAL.Models;
using Server.Infastructure.Exceptions;

namespace Server.DAL;

public class AuthRepository : IAuthRepository
{
    private readonly ServerDbContext dbContext;

    public AuthRepository(ServerDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task CreateSessionAsync(DbSession session)
    {
        await dbContext.DbSession.AddAsync(session);

        await dbContext.SaveChangesAsync();
    }

    public async Task CreateUserTokenAsync(UserToken userToken)
    {
        await dbContext.UserTokens.AddAsync(userToken);

        await dbContext.SaveChangesAsync();
    }

    public async Task<bool> IsSessionValidAsync(Guid sessionId)
    {
        var session = await dbContext.DbSession
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.SessionId == sessionId && s.UserId != null);


        if (session == null)
        {
            throw new NotFoundException("Session was not found");
        }

        if (session.User.Status == AccountStatus.Blocked)
        {
            throw new UserBlockedException("User is blocked");
        }

        session.User.LastActivity = DateTime.UtcNow;
        await dbContext.SaveChangesAsync();

        return true;
    }

    public async Task DeleteSessionAsync(Guid sessionId)
    {
        var sessionModel = await dbContext.DbSession.FirstOrDefaultAsync(s => s.SessionId == sessionId);

        if (sessionModel == null)
        {
            throw new NotFoundException("Session was not found");
        }

        dbContext.DbSession.Remove(sessionModel);
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteUserTokenAsync(Guid userToken)
    {
        var userTokenModel = await dbContext.UserTokens.FirstOrDefaultAsync(x => x.UserTokenId == userToken);

        if (userTokenModel == null)
        {
            throw new NotFoundException("UserToken was not found");
        }

        dbContext.UserTokens.Remove(userTokenModel);
        await dbContext.SaveChangesAsync();
    }
}