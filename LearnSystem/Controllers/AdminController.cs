using LearnSystem.Models.ModelsDTO;
using LearnSystem.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceStatusResult;

namespace LearnSystem.Controllers
{
    [Authorize(Roles ="admin")]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AdminController(IAdminService adminService) : ControllerBase
    {
        [HttpPut]
        public async Task<ActionResult> UpdateRole(ChangeRoleUserDto changeRoleUserDto)
            => await FromServiceResultBaseAsync(adminService.UpdateRoleUser(changeRoleUserDto));

        [HttpDelete]
        public async Task<ActionResult> DeleteUser([FromBody] string userId)
            => await FromServiceResultBaseAsync(adminService.DeleteUser(userId));

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
