using LearnSystem.Models;
using LearnSystem.Models.ModelsDTO;
using LearnSystem.Services.IServices;
using Microsoft.AspNetCore.Identity;
using ServiceStatusResult;
using System.Security.Claims;

namespace LearnSystem.Services;

public class RoleService(
    RoleManager<Roles> roleManager,
    UserManager<User> userManager
    ) : IRoleService
{
    public async Task<ServiceResultBase<bool>> AddRole(string roleName)
    {
        var role = roleManager.FindByNameAsync(roleName);
        if (role == null)
            return new NotFoundServiceResult<bool>();

        var result = await roleManager.CreateAsync(new Roles { Name = roleName });

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

    public async Task<ServiceResultBase<List<Roles>>> GetAllRoles()
    {
        var roles = roleManager.Roles.ToList();

        return new OkServiceResult<List<Roles>>(roles);
    }

    public async Task<ServiceResultBase<IList<string>>> MyRoles(string id)
    {

        var user = await userManager.FindByIdAsync(id);
        var roles = await userManager.GetRolesAsync(user);

        return new OkServiceResult<IList<string>>(roles);
    }
}

