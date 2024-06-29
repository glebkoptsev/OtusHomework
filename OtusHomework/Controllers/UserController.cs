using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OtusHomework.DTOs;
using OtusHomework.Services;

namespace OtusHomework.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController(UserService userService) : ControllerBase
    {
        private readonly UserService userService = userService;

        [HttpPost, Route("register")]
        public async Task<ActionResult<UserRegisterResponse>> Register(UserRegisterRequest request)
        {
            return Ok(await userService.RegisterUserAsync(request));
        }

        [HttpGet, Route("get/{id}"), Authorize]
        public async Task<ActionResult<User>> GetUser(Guid id)
        {
            var user = await userService.GetUserAsync(id);
            if (user is null) return NotFound();
            return Ok(user);
        }
    }
}
