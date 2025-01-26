using Microsoft.AspNetCore.Mvc;
using Server.BL.Interfaces;
using Server.Infastructure.Middlewares;

namespace Server.Services
{
    [Route("/user/")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> logger;
        private readonly IUserService userService;

        public UserController(ILogger<UserController> logger, IUserService userService)
        {
            this.logger = logger;
            this.userService = userService;
        }

        [AuthTokenRequired]
        [HttpGet]
        [Route("users")]
        public async Task<IActionResult> UsersToList([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            pageSize = Math.Min(pageSize, 100);

            var usersResponse = await userService.GetUsersToListAsync(pageNumber, pageSize);

            return Ok(usersResponse);
        }

        [AuthTokenRequired]
        [HttpPost]
        [Route("users")]
        public async Task<IActionResult> BlockUsers(UsersIdDto usersId)
        {
            if (usersId == null || usersId.UsersId.Count == 0)
            {
                return BadRequest("No user IDs provided.");
            }

            await userService.BlockUsersAsync(usersId.UsersId);
            return Ok("Users successfully blocked.");
        }

        [AuthTokenRequired]
        [HttpPatch]
        [Route("users")]
        public async Task<IActionResult> UnblockUsers(UsersIdDto usersId)
        {
            if (usersId == null || usersId.UsersId.Count == 0)
            {
                return BadRequest("No user IDs provided.");
            }

            await userService.UnblockUsersAsync(usersId.UsersId);
            return Ok("Users successfully unblocked.");
        }

        [AuthTokenRequired]
        [HttpDelete]
        [Route("users")]
        public async Task<IActionResult> DeleteUsers(UsersIdDto usersId)
        {
            if (usersId == null || usersId.UsersId.Count == 0)
            {
                return BadRequest("No user IDs provided.");
            }

            await userService.DeleteUsersAsync(usersId.UsersId);
            return Ok("Users successfully deleted.");
        }
    }

    public record UsersIdDto(List<int> UsersId);

    public record UserDto(int Id, string FullName, string Email, string LastSeen, string Status);
    public record UsersToListResponse(List<UserDto> Users, int TotalCount, int PageNumber, int PageSize);
}
