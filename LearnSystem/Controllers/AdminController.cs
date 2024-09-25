using LearnSystem.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceStatusResult;

namespace LearnSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize(Roles ="admin")]
    public class AdminController(IAdminService adminService) : ControllerBase
    {

        //[HttpGet]
        //public async Task<ActionResult> GetAllUsers() =>
        //    await FromServiceResultBaseAsync(adminService.GetUsers());



        protected async Task<ActionResult> FromServiceResultBaseAsync<T>(Task<ServiceResultBase<T>> task)
        {
            var result = await task;

            if (result.StatusCode < 400)
            {
                return StatusCode(result.StatusCode, result.Result);
            }

            if (result == null)
            {
                return NoContent();
            }

            return StatusCode(result.StatusCode, result.StatusMessage);

        }
            

    }
}
