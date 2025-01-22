using Microsoft.AspNetCore.Mvc;

namespace Server.Services
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserService : ControllerBase
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
