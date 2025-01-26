using Server.BL.Interfaces;
using Server.BL.Models;
using Server.DAL.Interfaces;
using Server.DAL.Models;
using Server.Infastructure.Exceptions;
using Server.Services;

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

    public SessionInformation GetSessionInformation(HttpContext context)
    {
        var userAgent = context?.Request?.Headers["User-Agent"].ToString();
        var ipAddress = context?.Connection?.RemoteIpAddress?.ToString();

        return new SessionInformation()
        {
            IpAddress = ipAddress,
            UserAgent = userAgent
        };
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

    public async Task<LoginResponse> Authorization(string email, string password, bool rememberMe, string device, string ipAddress)
    {
        logger.LogInformation("Try to get appUser by email");
        AppUser? appUser = await userRepository.GetAppUser(email);

        if (appUser == null)
        {
            logger.LogWarning("appUser was null");
            throw new UserNotFoundException("User with that email was not found");
        }
        logger.LogInformation("Successfully gave appUser");

        logger.LogInformation("Hashing password");
        string guessHashedPassword = encrypt.HashPassword(password, appUser.Salt.ToString());
        logger.LogInformation("Successfully hashed password");

        if (appUser.Password != guessHashedPassword)
        {
            logger.LogWarning("Password was incorrect");
            throw new IncorrectPasswordException("Password is incorrect");
        }
        logger.LogInformation("Successfully hashed password");

        logger.LogInformation("Try to get userId ");
        int? userId = await userRepository.GetUserId(appUser.Id);

        if (userId == null)
        {
            logger.LogWarning("User was not founded");
            throw new UserNotFoundException("User with that id doesn`t have access");
        }
        logger.LogInformation("Successfully  user was founded");

        var session = Guid.NewGuid();


        logger.LogInformation("Creating Session for new login");
        await authRepository.CreateSession(new DbSession()
        {
            UserId = userId,
            SessionId = session,
            Device = device,
            IpAddress = ipAddress
        });
        logger.LogInformation("Success! Session was created");

        Guid? rememberMeValue = null;

        
        if (rememberMe)
        {
            logger.LogInformation("RememberMe was true, creating new userToken");
            rememberMeValue = Guid.NewGuid();

            await authRepository.CreateUserToken(new UserToken()
            {
                UserId = (int)userId,
                UserTokenId = (Guid)rememberMeValue
            });

            logger.LogInformation("UserToken was created");
        }

        return new LoginResponse(session.ToString(), rememberMeValue.ToString());
    }

    public async Task<bool> ValidateToken(Guid sessionId)
    {
        logger.LogInformation("Validating sessionId in database");
        bool isValid = await authRepository.IsSessionValid(sessionId);

        return isValid;
    }

    public async Task<Guid> CheckUserToken(Guid userTokenId, string device, string ipAddress)
    {
        logger.LogInformation("Try to find userToken in database");
        var userToken = await userRepository.GetUserToken(userTokenId);

        Guid sessionId = Guid.NewGuid();

        logger.LogInformation("Creating new session in db");

        await authRepository.CreateSession(new DbSession()
        {
            SessionId = sessionId,
            UserId = userToken.UserId,
            Device = device,
            IpAddress = ipAddress
        });

        return sessionId;
    }
}