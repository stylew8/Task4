using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Server.BL.Interfaces;
using Server.Infastructure.Middlewares;

namespace Server.Services
{
    [Route("/auth/")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService authService;

        public AuthController(IAuthService authService)
        {
            this.authService = authService;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Registration(RegisterRequest request)
        {
            int userId = await authService.Registration(
                string.Concat(request.Name, " ", request.Surname),
                request.Email,
                request.Password);

            var sessionInfo = authService.GetSessionInformation(HttpContext);

            var sessionId = await authService.CreateSession(
                sessionInfo.IpAddress,
                sessionInfo.UserAgent,
                userId);

            return Ok(new RegisterResponse(sessionId));
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var sessionInfo = authService.GetSessionInformation(HttpContext);

            var response = await authService.Authorization(
                request.Email,
                request.Password,
                request.RememberMe,
                sessionInfo.UserAgent,
                sessionInfo.IpAddress ?? "Not found");

            return Ok(response);
        }

        [AuthTokenRequired]
        [HttpPost]
        [Route("validate/session")]
        public async Task<IActionResult> ValidateSession()
        {
            return Ok();
        }

        [HttpPost]
        [Route("validate/userToken")]
        public async Task<IActionResult> ValidateUserToken(UserTokenRequest request)
        {
            var sessionInfo = authService.GetSessionInformation(HttpContext);

            Guid sessionId =
                await authService.CheckUserToken(request.userToken, sessionInfo.UserAgent, sessionInfo.IpAddress);

            return Ok(new UserTokenResponse(sessionId));
        }
    }

    public record UserTokenRequest(Guid userToken);

    public record UserTokenResponse(Guid sessionId);

    public record LoginRequest(string Email, string Password, bool RememberMe = false);
    public record LoginResponse(string SessionId, string? RememberMe);

    public record RegisterRequest(string Name, string Surname, string Email, string Password);
    public record RegisterResponse(Guid SessionId);
}
