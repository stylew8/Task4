using Server.DAL.Models;

namespace Server.DAL.Interfaces;

public interface IUserRepository
{
    Task<int> CreateAppUser(AppUser appUser);
    Task<int> CreateUser(User user);
}