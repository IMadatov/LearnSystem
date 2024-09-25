using AutoMapper;
using LearnSystem.Models;
using LearnSystem.Models.ModelsDTO;
using LearnSystem.Services.IServices;
using Microsoft.AspNetCore.Identity;
using ServiceStatusResult;

namespace LearnSystem.Services;

public class AdminService(
    UserManager<User> userManager,
    RoleManager<Roles> roleManager,
    IMapper autoMapper
    ) : IAdminService
{
    public async Task<ServiceResultBase<List<UserDto>>> GetUsers()
    {
        var users = userManager.Users.ToList();

        var usersDto = autoMapper.Map<List<UserDto>>( users );

        return new OkServiceResult<List<UserDto>>(usersDto);
    }

    
}
