using LearnSystem.DbContext;
using LearnSystem.Models;
using LearnSystem.Models.ModelsDTO;
using LearnSystem.Services.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ServiceStatusResult;

namespace LearnSystem.Services;


public class UserService(
    ApplicationDbContext _dbContext,
    IHttpContextAccessor _httpContextAccessor,
    UserManager<User> userManager
    ) : IUserService
{
    public async Task<ServiceResultBase<List<User>>> GetUsers()
    {
        return new OkServiceResult<List<User>>(await _dbContext.Users.ToListAsync());
    }

    public async Task<ServiceResultBase<UserDto>> Me(string userId)
    {

        var user =await userManager.FindByIdAsync(userId );


        if (user == null)
        {
            return new NotFoundServiceResult<UserDto>();
        }

        var roles = await userManager.GetRolesAsync(user);

        var userDto = new UserDto
        {
            Id=user.Id,
            TelegramId = user.TelegramId!,
            CreatedAt=user.CreatedAt,
            Email=user.Email!,
            FirstName=user.FirstName!,
            LastName =  user.LastName!,
            PhotoUrl=user.PhotoUrl!,
            Roles= roles,
            TelegramUserName=user.TelegramUserName!,
            UserName= user.UserName!

            
        };
        
        return new OkServiceResult<UserDto>("profile", userDto);
    }
}
