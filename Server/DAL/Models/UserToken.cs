using System.ComponentModel.DataAnnotations;

namespace Server.DAL.Models;

public class UserToken
{
    [Key]
    public Guid UserTokenId { get; set; }
    public User User { get; set; }
    public int UserId { get; set; }

}