using Server.Services;

namespace Server.BL.Interfaces;

public interface IUserService
{
    Task<UsersToListResponse> GetUsersToListAsync(int pageNumber, int pageSize);
    Task BlockUsersAsync(List<int> userIds);
    Task UnblockUsersAsync(List<int> userIds);
    Task DeleteUsersAsync(List<int> userIds);
}