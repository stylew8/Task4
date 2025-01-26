
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Server.BL;
using Server.BL.Interfaces;
using Server.DAL;
using Server.DAL.Interfaces;
using Server.DAL.Models;
using Server.Infastructure;

namespace Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.WebHost.ConfigureKestrel(options =>
            {
                options.ListenAnyIP(8080);
                options.ListenAnyIP(7156, listenOptions =>
                {
                    listenOptions.UseHttps("/app/ssl/cert.pem", "/app/ssl/cert.key");  // Пути к сертификатам, которые монтируются в контейнер
                });
            });


            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.SetMinimumLevel(LogLevel.Warning);

            // Add services to the container.
            builder.Services.AddLogging();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddSingleton<IEncrypt, Pbkdf2Encryptor>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IAuthRepository, AuthRepository>();

            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
            builder.Services.AddProblemDetails();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<ServerDbContext>(options =>
            {
                options.UseMySQL(builder.Configuration.GetConnectionString("Default") ?? "");
            });

            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.WithOrigins("http://localhost:3000", "http://uniqum.school")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });


            var app = builder.Build();

            app.UseCors();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseExceptionHandler();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    
}
}
