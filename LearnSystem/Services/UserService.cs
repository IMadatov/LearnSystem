using AutoMapper;
using BaseCrud.EntityFrameworkCore;
using LearnSystem.DbContext;
using LearnSystem.Models;
using LearnSystem.Models.ModelsDTO;
using LearnSystem.Services.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ServiceStatusResult;

namespace LearnSystem.Services;


public class UserService(
    ApplicationDbContext _context,
    IHttpContextAccessor _httpContextAccessor,
    UserManager<User> userManager,
    IMapper autoMapper,
    RoleManager<ApplicationRole> roleManager
) : IUserService
{
    public async Task<ServiceResultBase<UserDto>> Me(Guid userId)
    {

        var user = await userManager.FindByIdAsync(userId.ToString());

        user = await _context.Users.Include(x => x.StatusUser).FirstOrDefaultAsync(x => x.Id == user.Id);

        if (user == null)
        {
            return new NotFoundServiceResult<UserDto>();
        }

        var roles = await userManager.GetRolesAsync(user);
        var status = user.StatusUser;

        var userDto = autoMapper.Map<UserDto>(user);
        userDto.Roles = roles;
        userDto.StatusUser = autoMapper.Map<StatusUserDto>(status);

        return new OkServiceResult<UserDto>("profile", userDto);
    }

    public async Task<ServiceResultBase<bool>> Refresh()
    {

        return new OkServiceResult<bool>(true);
    }
}

