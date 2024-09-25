using LearnSystem.Models;
using LearnSystem.Models.ModelsDTO;
using LearnSystem.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ServiceStatusResult;
using System.Security.Claims;

namespace LearnSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]

    public class UserController(IUserService userService) : ControllerBase
    {

        [HttpGet]
        [Authorize(Roles="admin")]
        public async Task<ActionResult> GetAllUser() =>
            await FromServiceResultBaseAsync(userService.GetUsers());

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
