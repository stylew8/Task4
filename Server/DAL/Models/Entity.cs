using System.ComponentModel.DataAnnotations;

namespace Server.DAL.Models;

public class Entity
{
    [Key]
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;
}