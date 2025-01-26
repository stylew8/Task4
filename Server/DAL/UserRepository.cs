using Microsoft.EntityFrameworkCore;
using Server.DAL.Interfaces;
using Server.DAL.Models;
using Server.Infastructure.Exceptions;

namespace Server.DAL;

public class UserRepository : IUserRepository
{
    private readonly ServerDbContext dbContext;

    public UserRepository(ServerDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<int> CreateAppUser(AppUser appUser)
    {
        await dbContext.AppUsers.AddAsync(appUser);

        await dbContext.SaveChangesAsync();

        return appUser.Id;
    }

    public async Task<int> CreateUser(User user)
    {
        await dbContext.Users.AddAsync(user);

        await dbContext.SaveChangesAsync();

        return user.Id;
    }

    public async Task<AppUser?> GetAppUser(string email)
    {
        return await dbContext.AppUsers.FirstOrDefaultAsync(x => x.Email == email);
    }

    public async Task<int?> GetUserId(int appUserId)
    {
        User? user = await dbContext.Users.FirstOrDefaultAsync(x => x.AppUserId == appUserId);

        if (user == null)
        {
            return null;
        }

        if (user.Status == AccountStatus.Blocked)
        {
            throw new UserBlockedException("User is blocked");
        }

        user.LastActivity = DateTime.UtcNow;
        await dbContext.SaveChangesAsync();

        return user.Id;
    }

    public async Task<UserToken> GetUserToken(Guid userTokenId)
    {
        var userToken = await dbContext.UserTokens.FirstOrDefaultAsync(x => x.UserTokenId == userTokenId);

        if (userToken == null)
        {
            throw new UserNotFoundException("UserToken was not found");
        }

        return userToken;
    }
}