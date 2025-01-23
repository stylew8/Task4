using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Runtime.CompilerServices;
using MySql.Data.MySqlClient;

namespace Server.Infastructure;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IProblemDetailsService problemDetailsService;

    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger,
        IProblemDetailsService problemDetailsService)
    {
        _logger = logger;
        this.problemDetailsService = problemDetailsService;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken
        )
    {
        httpContext.Response.ContentType = "application/problem+json";

        if (exception is DbUpdateException dbUpdateException) 
            // тут думал как лучше сделать, словить в самом DAL и выкинуть свою ошибку сюда либо вот так как сделал.
        {
            var innerException = dbUpdateException.InnerException;
            if (innerException is MySqlException sqlException)
            {
                if (sqlException.Number == 1062)
                {
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

        return true;
    }
}