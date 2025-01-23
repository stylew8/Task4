using Server.DAL.Interfaces;
using Server.DAL.Models;

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
}