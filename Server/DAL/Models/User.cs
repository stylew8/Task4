namespace Server.DAL.Models;

public class User : Entity
{
    public required string FullName { get; set; }

    public AppUser AppUser { get; set; }
    public required int AppUserId { get; set; }
    public required AccountStatus Status { get; set; }
    public required DateTime LastActivity { get; set; } = DateTime.UtcNow;
}

public enum AccountStatus
{
    Active, Blocked
}