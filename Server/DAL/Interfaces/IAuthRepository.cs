using Server.DAL.Models;

namespace Server.DAL.Interfaces;

public interface IAuthRepository
{
    Task CreateSession(DbSession session);
}