using Server.DAL.Models;

namespace Server.DAL.Interfaces;

public interface IAuthRepository
{
    Task CreateSession(DbSession session);
    Task CreateUserToken(UserToken userToken);
    Task<bool> IsSessionValid(Guid sessionId);
}