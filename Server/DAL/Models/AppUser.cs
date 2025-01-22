using Microsoft.EntityFrameworkCore;

namespace Server.DAL.Models;

[Index(nameof(Email), IsUnique = true)]
public class AppUser : Entity
{
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string Salt { get; set; }
}