using LearnSystem.DbContext;
using LearnSystem.Models;
using LearnSystem.Models.ModelsDTO;
using LearnSystem.Services.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ServiceStatusResult;
using System.Security.Claims;

namespace LearnSystem.Services;

public class RoleService(
    RoleManager<ApplicationRole> roleManager,
    UserManager<User> userManager,
    ApplicationDbContext context
    ) : IRoleService
{
    public async Task<ServiceResultBase<bool>> AddRole(string roleName)
    {
        var role = roleManager.FindByNameAsync(roleName);
        if (role == null)
            return new NotFoundServiceResult<bool>();

        var result = await roleManager.CreateAsync(new ApplicationRole { Name = roleName });

        if (result.Succeeded)
        {
            return new OkServiceResult<bool>(true);
        }
        return new OkServiceResult<bool>(false);
    }

    public async Task<ServiceResultBase<bool>> AddUserToRole(ToRoleDto toRoleDto)
    {
        var user = await userManager.FindByIdAsync(toRoleDto.UserId);
        if (user == null)
        {
            return new BadRequesServiceResult<bool>(false);
        }

        var result = await userManager.AddToRoleAsync(user, toRoleDto.Role);

        if (result.Succeeded)
        {
           
            return new OkServiceResult<bool>(true);
        }
        return new OkServiceResult<bool>(false);
    }

    public async Task<ServiceResultBase<List<ApplicationRole>>> GetAllRoles()
    {
        var roles = roleManager.Roles.ToList();

        return new OkServiceResult<List<ApplicationRole>>(roles);
    }

    public async Task<ServiceResultBase<IList<string>>> MyRoles(string id)
    {

        var user = await userManager.FindByIdAsync(id);
        var roles = await userManager.GetRolesAsync(user);

        return new OkServiceResult<IList<string>>(roles);
    }
}

