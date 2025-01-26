using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Runtime.CompilerServices;
using MySql.Data.MySqlClient;
using Server.Infastructure.Exceptions;

namespace Server.Infastructure;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> logger;
    private readonly IProblemDetailsService problemDetailsService;

    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger,
        IProblemDetailsService problemDetailsService)
    {
        this.logger = logger;
        this.problemDetailsService = problemDetailsService;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken
        )
    {
        httpContext.Response.ContentType = "application/problem+json";

        if (exception is IncorrectPasswordException ex)
        {
            logger.LogWarning($"Incorrect password: {ex.Title}");

            httpContext.Response.StatusCode = ex.StatusCode;

            return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext()
            {
                HttpContext = httpContext,
                ProblemDetails = new ProblemDetails()
                {
                    Status = ex.StatusCode,
                    Title = "Incorrect password",
                    Detail = ex.Title
                }
            });
        }

        if (exception is UserBlockedException blockedEx)
        {
            logger.LogWarning($"User is blocked: {blockedEx.Message}");

            httpContext.Response.StatusCode = blockedEx.StatusCode;

            return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext()
            {
                HttpContext = httpContext,
                ProblemDetails = new ProblemDetails()
                {
                    Title = "User is blocked",
                    Status = blockedEx.StatusCode,
                    Detail = blockedEx.Title
                }
            });
        }

        if (exception is UserNotFoundException ue)
        {
            logger.LogWarning($"Not Found: {ue.Title}");

            httpContext.Response.StatusCode = ue.StatusCode;

            return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext()
            {
                HttpContext = httpContext,
                ProblemDetails = new ProblemDetails()
                {
                    Status = ue.StatusCode,
                    Title = "User was not found",
                    Detail = ue.Title
                }
            });
        }

        if (exception is DbUpdateException dbUpdateException) 
            // тут думал как лучше сделать, словить в самом DAL и выкинуть свою ошибку сюда либо вот так как сделал.
        {
            
            var innerException = dbUpdateException.InnerException;
            if (innerException is MySqlException sqlException)
            {
                logger.LogError($"Mysqlexception : {sqlException.Message}");
                if (sqlException.Number == 1062)
                {
                    logger.LogWarning("Duplicate of insert in database");

                    httpContext.Response.StatusCode = (int)HttpStatusCode.Conflict;

                    return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext()
                    {
                        HttpContext = httpContext,
                        ProblemDetails = new ProblemDetails()
                        {
                            Status = (int)HttpStatusCode.Conflict,
                            Title = "Dublicate of email",
                            Detail = "User with same email already exist"
                        }
                    });
                }
            }
        }
        else
        {
            logger.LogError($"Unexpected exception: {exception.Message}");

            httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext()
            {
                HttpContext = httpContext,
                ProblemDetails = new ProblemDetails()
                {
                    Status = (int)HttpStatusCode.BadRequest,
                    Title = "Problems with api",
                    Detail = "Now we have some problems with api. Try again later.."
                }
            });
        }

        return true;
    }
}