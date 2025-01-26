using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Server.BL.Interfaces;
using Server.Infastructure.Exceptions;

namespace Server.Infastructure.Middlewares;


[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public class AuthTokenRequired : Attribute, IAsyncAuthorizationFilter
{
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var service = context.HttpContext.RequestServices.GetService<IAuthService>();

        if (service == null)
        {
            context.Result = new StatusCodeResult(500); // Internal Server Error
            return;
        }

        if (!context.HttpContext.Request.Headers.ContainsKey("Authorization"))
        {
            context.Result = new UnauthorizedObjectResult("Authorization token was not found");
            return;
        }

        var token = context.HttpContext.Request.Headers["Authorization"].ToString().Trim();

        if (string.IsNullOrEmpty(token))
        {
            context.Result = new UnauthorizedObjectResult("Authorization token was not found");
            return;
        }

        if (!Guid.TryParse(token, out Guid session) || !await service.ValidateToken(session))
        {
            context.Result = new UnauthorizedObjectResult("Authorization session is not valid");
            return;
        }
    }
}