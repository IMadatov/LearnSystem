using LearnSystem.Services;
using LearnSystem.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceStatusResult;

namespace LearnSystem.Controllers
{
    [Authorize]
    public class TelegramBotController(ITelegramService telegramService) : 
        BaseController
    {
        [HttpGet]
        public async Task<ActionResult> getUser()
        {
            var username=await  telegramService.UserProfilePhotos(new Models.User { TelegramUserName = "islam_madatov", TelegramId = "629848052" });

            return Ok(username);
        }

        [HttpGet]
        public async Task<ActionResult> RefreshUserInfo()=>
            await FromServiceResultBaseAsync(telegramService.RefreshUserInfo());    

        protected async Task<ActionResult> FromServiceResultBaseAsync<T>(Task<ServiceResultBase<T>> task)
        {
            var result = await task;

            if (result == null)
            {
                return NoContent();
            }
            var isOk = result.StatusCode < 400;

            if (isOk) 
                return StatusCode(result.StatusCode,result.Result);
            
            return StatusCode(result.StatusCode, "Request failed");
        }

             
    }
}
