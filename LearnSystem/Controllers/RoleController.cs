using LearnSystem.Models.ModelsDTO;
using LearnSystem.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceStatusResult;
using System.Security.Claims;

namespace LearnSystem.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
[Authorize]
public class RoleController(IRoleService roleService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult> MyRoles()
    {
        var id = HttpContext.User.Claims.FirstOrDefault(x=>x.Type==ClaimTypes.NameIdentifier).Value;

        return await FromServiceResultBaseAsync(roleService.MyRoles(id));  
    }

    [HttpPost]
    public async Task<ActionResult> AddRole(string roleName)
    {
        return await FromServiceResultBaseAsync(roleService.AddRole(roleName));
    }

    [HttpGet]
    public async Task<ActionResult> GetAllRoles()
        => await FromServiceResultBaseAsync(roleService.GetAllRoles());

    [HttpPost]
    public async Task<ActionResult> AddUserToRole(ToRoleDto toRoleDto)
    {
        return await FromServiceResultBaseAsync(roleService.AddUserToRole(toRoleDto));
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
