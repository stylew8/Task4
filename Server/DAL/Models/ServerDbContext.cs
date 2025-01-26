using Microsoft.EntityFrameworkCore;

namespace Server.DAL.Models;

public class ServerDbContext : DbContext
{
    public ServerDbContext(DbContextOptions<ServerDbContext> options):base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<DbSession> DbSession { get; set; }
    public DbSet<AppUser> AppUsers { get; set; }
    public DbSet<UserToken> UserTokens { get; set; }

}