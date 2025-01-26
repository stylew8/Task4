using System.Runtime.CompilerServices;
using Server.BL.Models;
using Server.Services;

namespace Server.BL.Interfaces;

public interface IAuthService
{

    SessionInformation GetSessionInformation(HttpContext context);
    Task<int> RegistrationAsync(string FullName, string Email, string Password);
    Task<Guid> CreateSessionAsync(string ipAddress, string device, int? userId);
    Task<LoginResponse> AuthorizationAsync(string email, string password, bool rememberMe, string userAgent, string ipAddress);
    Task<bool> ValidateTokenAsync(Guid sessionId);
    Task<Guid> CheckUserTokenAsync(Guid userToken, string device, string ipAddress);
    Task LogoutAsync(Guid sessionId, Guid? rememberMe);
}