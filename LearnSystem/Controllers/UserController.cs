using LearnSystem.Services;
using LearnSystem.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceStatusResult;
using System.Security.Claims;

namespace LearnSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class UserController(IUserService userService) : ControllerBase
    {

        [HttpGet]
        public async Task<ActionResult> Refresh() =>
            await FromServiceResultBaseAsync(userService.Refresh());

        [HttpGet]
        public async Task<ActionResult> Me()
        {
            var user = HttpContext.User.ToString();
            Console.WriteLine(user);

            var userId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            return await FromServiceResultBaseAsync(userService.Me(userId));
        }

        [HttpPost]
        public async Task<ActionResult> SignOut()
        {
            await HttpContext.SignOutAsync();
            return Ok(true);
        }


        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> GetAllUsers([FromQuery] int First, [FromQuery] int Rows) =>
            await FromServiceResultBaseAsync(userService.GetUsers(First, Rows));

        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> GetAdmins([FromQuery] int First, [FromQuery] int Rows) =>
            await FromServiceResultBaseAsync(userService.GetAdmins(First, Rows));

        [HttpGet]
        [Authorize(Roles = "admin, teacher")]
        public async Task<ActionResult> GetTeachers([FromQuery] int First, [FromQuery] int Rows) =>
            await FromServiceResultBaseAsync(userService.GetTeachers(First, Rows));

        [HttpGet]
        [Authorize(Roles = "admin,teacher")]
        public async Task<ActionResult> GetStudents([FromQuery] int First, [FromQuery] int Rows) =>
            await FromServiceResultBaseAsync(userService.GetStudents(First, Rows));


        protected async Task<ActionResult> FromServiceResultBaseAsync<T>(Task<ServiceResultBase<T>> task)
        {
            var result = await task;
            if (result == null)
            {
                return NoContent();
            }
            var isOk = result.StatusCode < 400;
            if (isOk)
            {
                return StatusCode(result.StatusCode, result.Result);
            }
            return StatusCode(result.StatusCode, "Request failed");
        }
    }
}
