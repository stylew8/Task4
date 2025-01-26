using Microsoft.EntityFrameworkCore;
using Server.BL.Interfaces;
using Server.DAL;
using Server.DAL.Interfaces;
using Server.Services;

namespace Server.BL;

public class UserService : IUserService
{
    private readonly ILogger<UserService> logger;
    private readonly IUserRepository userRepository;

    public UserService(ILogger<UserService> logger, IUserRepository userRepository)
    {
        this.logger = logger;
        this.userRepository = userRepository;
    }

    public async Task<UsersToListResponse> GetUsersToListAsync(int pageNumber, int pageSize)
    {
        logger.LogInformation($"Fetching users: PageNumber={pageNumber}, PageSize={pageSize}");

        var pagedUsers = await userRepository.GetUsersAsync(pageNumber, pageSize);

        List<UserDto> usersDto = new List<UserDto>();

        logger.LogInformation("Formatting users into correct format");
        pagedUsers.Users.ForEach(x =>
        {
            var formattedLastActivity = FormatLastActivity(x.LastActivity);
            usersDto.Add(new UserDto(
                x.Id,
                x.FullName,
                x.AppUser.Email,
                formattedLastActivity,
                x.Status.ToString()
            ));
        });

        return new UsersToListResponse(
            Users: usersDto,
            TotalCount: pagedUsers.TotalCount,
            PageNumber: pageNumber,
            PageSize: pageSize
        );
    }

    private string FormatLastActivity(DateTime? lastActivity)
    {
        if (!lastActivity.HasValue)
            return "No activity";

        var timeDifference = DateTime.UtcNow - lastActivity.Value;

        if (timeDifference.TotalMinutes < 1)
            return "less than a minute ago";
        if (timeDifference.TotalMinutes < 60)
            return $"{(int)timeDifference.TotalMinutes} minutes ago";
        if (timeDifference.TotalHours < 24)
            return $"{(int)timeDifference.TotalHours} hours ago";
        if (timeDifference.TotalDays < 7)
            return $"{(int)timeDifference.TotalDays} days ago";

        return lastActivity.Value.ToString("MMM dd, yyyy");
    }

    public async Task BlockUsersAsync(List<int> userIds)
    {
        logger.LogInformation("Try to block users");
        await userRepository.BlockUsersAsync(userIds);
    }

    public async Task UnblockUsersAsync(List<int> userIds)
    {
        logger.LogInformation("Try to unblock users");
        await userRepository.UnblockUsersAsync(userIds);
    }

    public async Task DeleteUsersAsync(List<int> userIds)
    {
        logger.LogInformation("Try to delete users");
        await userRepository.DeleteUsersAsync(userIds);
    }
}