namespace Server.BL.Interfaces;

public interface IAuthService
{
    Task<int> Registration(string FullName, string Email, string Password);
    Task<Guid> CreateSession(string ipAddress, string device, int? userId);
}