using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Server.DAL.Models;

[Index(nameof(UserTokenId))]
public class UserToken
{
    [Key]
    public Guid UserTokenId { get; set; }
    public User User { get; set; }
    public int UserId { get; set; }

}