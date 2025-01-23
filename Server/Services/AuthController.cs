using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Server.BL.Interfaces;

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

            var userAgent = HttpContext?.Request?.Headers["User-Agent"];
            var ipAddress = HttpContext?.Connection?.RemoteIpAddress?.ToString();

            var sessionId = await authService.CreateSession(
                ipAddress,
                userAgent,
                userId);

            return Ok(new RegisterResponse(sessionId));
        }
    }

    public record RegisterRequest(string Name, string Surname, string Email, string Password);
    public record RegisterResponse(Guid SessionId);
}
