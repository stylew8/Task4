using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.BL.Models;
using Server.DAL.Interfaces;
using Server.DAL.Models;
using Server.Infastructure.Exceptions;
using System.Linq;

namespace Server.DAL;

public class UserRepository : IUserRepository
{
    private readonly ServerDbContext dbContext;
    private readonly ILogger<UserRepository> logger;

    public UserRepository(ServerDbContext dbContext, ILogger<UserRepository> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }

    public async Task<int> CreateAppUserAsync(AppUser appUser)
    {
        await dbContext.AppUsers.AddAsync(appUser);

        await dbContext.SaveChangesAsync();

        return appUser.Id;
    }

    public async Task<int> CreateUserAsync(User user)
    {
        await dbContext.Users.AddAsync(user);

        await dbContext.SaveChangesAsync();

        return user.Id;
    }

    public async Task<AppUser?> GetAppUserAsync(string email)
    {
        return await dbContext.AppUsers.FirstOrDefaultAsync(x => x.Email == email);
    }

    public async Task<int?> GetUserIdAsync(int appUserId)
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

    public async Task<UserToken> GetUserTokenAsync(Guid userTokenId)
    {
        UserToken? userToken = await dbContext.UserTokens.FirstOrDefaultAsync(x => x.UserTokenId == userTokenId);

        if (userToken == null)
        {
            throw new UserNotFoundException("UserToken was not found");
        }

        User? user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == userToken.UserId);

        if (user == null)
        {
            throw new UserNotFoundException("User was not found");
        }

        if (user.Status == AccountStatus.Blocked)
        {
            throw new UserBlockedException("User is blocked");
        }

        user.LastActivity = DateTime.UtcNow;
        await dbContext.SaveChangesAsync();

        return userToken;
    }

    public async Task<PagedResult<User>> GetUsersAsync(int pageNumber, int pageSize)
    {
        var query = dbContext.Users.Include(x=>x.AppUser).AsQueryable();

        var totalCount = await query.CountAsync();
        var users = await query
            .Skip((pageNumber - 1) * pageSize) 
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<User>(users, totalCount);
    }

    public async Task BlockUsersAsync(List<int> userIds)
    {
        var users = await dbContext.Users.Where(u => userIds.Contains(u.Id)).ToListAsync();
        users.ForEach(u => u.Status = AccountStatus.Blocked);
        await dbContext.SaveChangesAsync();
    }

    public async Task UnblockUsersAsync(List<int> userIds)
    {
        var users = await dbContext.Users.Where(u => userIds.Contains(u.Id)).ToListAsync();
        users.ForEach(u => u.Status = AccountStatus.Active);
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteUsersAsync(List<int> userIds)
    {
        logger.LogInformation("Trying find all needed users");
        var users = await dbContext.Users
            .Include(x => x.AppUser)
            .Where(u => userIds.Contains(u.Id))
            .ToListAsync();

        logger.LogInformation("Trying remove all needed users");
        dbContext.Users.RemoveRange(users);

        logger.LogInformation("Trying remove all needed appUsers");
        dbContext.AppUsers.RemoveRange(users.Select(x => x.AppUser));

        logger.LogInformation("Trying get all needed sessions");
        var sessions = await dbContext.DbSession
            .Where(x => userIds.Contains((int)x.UserId))
            .ToListAsync();

        logger.LogInformation("Trying remove all needed sessions");
        dbContext.DbSession.RemoveRange(sessions);

        logger.LogInformation("trying get all needed userTokens");
        var userTokens = await dbContext.UserTokens
            .Where(x=> userIds.Contains((int)x.UserId))
            .ToListAsync();

        logger.LogInformation("Trying remove usertokens");
        dbContext.UserTokens.RemoveRange(userTokens);


        await dbContext.SaveChangesAsync();
    }
}