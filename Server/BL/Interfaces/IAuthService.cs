using System.Runtime.CompilerServices;
using Server.BL.Models;
using Server.Services;

namespace Server.BL.Interfaces;

public interface IAuthService
{

    SessionInformation GetSessionInformation(HttpContext context);
    Task<int> Registration(string FullName, string Email, string Password);
    Task<Guid> CreateSession(string ipAddress, string device, int? userId);
    Task<LoginResponse> Authorization(string email, string password, bool rememberMe, string userAgent, string ipAddress);
    Task<bool> ValidateToken(Guid sessionId);
    Task<Guid> CheckUserToken(Guid userToken, string device, string ipAddress);
}