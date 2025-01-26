using Server.BL.Models;
using Server.DAL.Models;

namespace Server.DAL.Interfaces;

public interface IUserRepository
{
    Task<int> CreateAppUserAsync(AppUser appUser);
    Task<int> CreateUserAsync(User user);
    Task<AppUser?> GetAppUserAsync(string email);
    Task<int?> GetUserIdAsync(int appUserId);
    Task<UserToken> GetUserTokenAsync(Guid userTokenId);
    Task<PagedResult<User>> GetUsersAsync(int pageNumber, int pageSize);
    Task BlockUsersAsync(List<int> userIds);
    Task UnblockUsersAsync(List<int> userIds);
    Task DeleteUsersAsync(List<int> userIds);
}