using Server.DAL.Interfaces;
using Server.DAL.Models;

namespace Server.DAL;

public class AuthRepository : IAuthRepository
{
    private readonly ServerDbContext dbContext;

    public AuthRepository(ServerDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task CreateSession(DbSession session)
    {
        await dbContext.DbSession.AddAsync(session);

        await dbContext.SaveChangesAsync();
    }
}