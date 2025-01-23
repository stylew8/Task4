using Server.BL.Interfaces;
using Server.DAL.Interfaces;
using Server.DAL.Models;

namespace Server.BL;

public class AuthService : IAuthService
{
    private readonly IAuthRepository authRepository;
    private readonly IUserRepository userRepository;
    private readonly IEncrypt encrypt;
    private readonly ILogger<AuthService> logger;

    public AuthService(
        IAuthRepository authRepository,
        IUserRepository userRepository,
        IEncrypt encrypt,
        ILogger<AuthService> logger
        )
    {
        this.authRepository = authRepository;
        this.userRepository = userRepository;
        this.encrypt = encrypt;
        this.logger = logger;
    }

    public async Task<int> Registration(string fullName, string email, string password)
    {
        Guid salt = Guid.NewGuid();

        logger.LogInformation("Hashing password");
        string hashedPassword = encrypt.HashPassword(password, salt.ToString());
        logger.LogInformation("Password was hashed successfully");

        logger.LogInformation("Creating appUser for registration");
        int appUserId = await userRepository.CreateAppUser(new AppUser()
        {
            Email = email,
            Password = hashedPassword,
            Salt = salt
        });
        logger.LogInformation("AppUser was created successfully");

        logger.LogInformation("Creating User in database");
        int userId = await userRepository.CreateUser(new User()
        {
            AppUserId = appUserId,
            FullName = fullName,
            Status = AccountStatus.Active,
            LastActivity = DateTime.UtcNow
        });
        logger.LogInformation("User was created successfully");

        return userId;
    }

    public async Task<Guid> CreateSession(string ipAddress, string device, int? userId)
    {
        Guid sessionId = Guid.NewGuid();

        logger.LogInformation("Creating Session in database");
        await authRepository.CreateSession(new DbSession()
        {
            IpAddress = ipAddress,
            UserId = userId,
            Device = device,
            SessionId = sessionId
        });
        logger.LogInformation("Session was created successfully");

        return sessionId;
    }
}