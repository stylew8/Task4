namespace Server.DAL.Models;

public class User : Entity
{
    public required string FullName { get; set; }

    public required AppUser AppUser { get; set; }
    public int AppUserId { get; set; }
    public required AccountStatus Status { get; set; }
    public required DateTime LastActivity { get; set; } = DateTime.UtcNow;
}

public enum AccountStatus
{
    Active, Blocked
}