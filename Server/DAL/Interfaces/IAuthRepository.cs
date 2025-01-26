using Server.DAL.Models;

namespace Server.DAL.Interfaces;

public interface IAuthRepository
{
    Task CreateSessionAsync(DbSession session);
    Task CreateUserTokenAsync(UserToken userToken);
    Task<bool> IsSessionValidAsync(Guid sessionId);
    Task DeleteSessionAsync(Guid sessionId);
    Task DeleteUserTokenAsync(Guid userToken);
}