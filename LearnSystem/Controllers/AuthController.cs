using LearnSystem.Models.ModelsDTO;
using LearnSystem.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ServiceStatusResult;

namespace LearnSystem.Controllers
{

    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult> SignUp(SignUpDto signUp) =>
            await FromServiceResultBaseAsync(authService.SignUpAsync(signUp));

        [HttpPost]
        public async Task<ActionResult> SignOut() =>
            await FromServiceResultBaseAsync(authService.SignOutAsync());

        [HttpPost]
        public async Task<ActionResult> SignInAsync(SignInDto signInDto) =>
            await FromServiceResultBaseAsync(authService.SignInAsync(signInDto));

        [HttpGet]
        [Authorize(Roles = "admin123")]
        public async Task<ActionResult<bool>> OnSite()
            => await FromServiceResultBaseAsync(authService.OnSite());


        [HttpPost]
        public async Task<ActionResult> SignUpWithTelegram(UserTeleramDTO userTelegram, string telegramData)
        {
            var token = HttpContext.User.Claims.ToList();
            return await FromServiceResultBaseAsync(authService.SignUpWithTelegram(userTelegram, telegramData));
        }


        [HttpPost]
        public async Task<ActionResult> CheckUser(string userTelegramData)
            => await FromServiceResultBaseAsync(authService.CheckUsername(userTelegramData));

        [HttpGet]
        public async Task<ActionResult> CheckUsername(string username)
            => await FromServiceResultBaseAsync(authService.CheckUsername(username));


        [HttpPost]
        public async Task<ActionResult> CheckTelegramData(string telegramData)
            => await FromServiceResultBaseAsync(authService.CheckTelegramData(telegramData));
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
            return StatusCode(result.StatusCode, result.StatusMessage);
        }
    }
}
