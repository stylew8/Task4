using System.ComponentModel.DataAnnotations.Schema;

namespace Server.DAL.Models;

public class DbSession : Entity
{
    public required Guid SessionId { get; set; }
    public required string IpAddress { get; set; }
    public required string Device { get; set; }

    public User? User { get; set; }
    public int? UserId { get; set; }
}