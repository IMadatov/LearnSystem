using AutoMapper;
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
    RoleManager<Roles> roleManager
    ) : IUserService
{

    public async Task<ServiceResultBase<PaginatedList<UserDto>>> GetAdmins(int first, int rows)
    {
        var alladmins = from admin in _context.Users
                        join status in _context.StatusUsers on admin.StatusUserId equals status.Id
                        join ur in _context.UserRoles on admin.Id equals ur.UserId
                        join role in _context.Roles on ur.RoleId equals role.Id
        where role.Name == "admin"
                        select new UserDto(autoMapper.Map<UserDto>(admin), autoMapper.Map<StatusUserDto>(status)) { Roles = new List<string> { role.Name } };

        var count = alladmins.Count();

        var admins = await alladmins.Skip(first).Take(rows).ToListAsync();


        var results = new PaginatedList<UserDto>(admins, count);

        return new OkServiceResult<PaginatedList<UserDto>>(results);
    }

    public async Task<ServiceResultBase<PaginatedList<UserDto>>> GetStudents(int first, int rows)
    {
        var allStudents = from student in _context.Users
                          join status in _context.StatusUsers on student.StatusUserId equals status.Id
                          join ur in _context.UserRoles on student.Id equals ur.UserId
                          join role in _context.Roles on ur.RoleId equals role.Id
                          where role.Name == "student"
                          select new UserDto(autoMapper.Map<UserDto>(student), autoMapper.Map<StatusUserDto>(status)) { Roles = new List<string> { role.Name } };

        var count = allStudents.Count();

        var students = await allStudents.Skip(first).Take(rows).ToListAsync();

        var result = new PaginatedList<UserDto>(students, count);

        return new OkServiceResult<PaginatedList<UserDto>>(result);
    }

    public async Task<ServiceResultBase<PaginatedList<UserDto>>> GetTeachers(int first, int rows)
    {
        var allTeacher = from teacher in _context.Users
                         join status in _context.StatusUsers on teacher.StatusUserId equals status.Id
                         join ur in _context.UserRoles on teacher.Id equals ur.UserId
                         join role in _context.Roles on ur.RoleId equals role.Id
                         where role.Name == "teacher"
                         select new UserDto(autoMapper.Map<UserDto>(teacher), autoMapper.Map<StatusUserDto>(status)) { Roles = new List<string> { role.Name } }; ;

        var count = allTeacher.Count();

        var teachers = await allTeacher.Skip(first).Take(rows).ToListAsync();


       

        var result = new PaginatedList<UserDto>(teachers, count);

        return new OkServiceResult<PaginatedList<UserDto>>(result);
    }

    public async Task<ServiceResultBase<PaginatedList<UserDto>>> GetUsers(int first, int rows)
    {

        var count = _context.Users.Count();

        var usersWithRole = from user in _context.Users
                            join status in _context.StatusUsers
                            on user.StatusUserId equals status.Id
                            join roleUser in _context.UserRoles
                            on user.Id equals roleUser.UserId
                            join role in _context.Roles
                            on roleUser.RoleId equals role.Id
                            select new UserDto(autoMapper.Map<UserDto>(user), autoMapper.Map<StatusUserDto>(status)) { Roles=new List<string> { role.Name} };



        var usersDto=await usersWithRole.Skip(first).Take(rows).ToListAsync();

       

        var result = new PaginatedList<UserDto>(usersDto, count);

        return new OkServiceResult<PaginatedList<UserDto>>(result);

    }

    public async Task<ServiceResultBase<UserDto>> Me(string userId)
    {

        var user = await userManager.FindByIdAsync(userId);


        if (user == null)
        {
            return new NotFoundServiceResult<UserDto>();
        }

        var roles = await userManager.GetRolesAsync(user);
        var status = await _context.StatusUsers.FirstOrDefaultAsync(x => x.Id == user.StatusUserId);

        var userDto = autoMapper.Map<UserDto>(user);
        userDto.Roles = roles;
        userDto.Status = autoMapper.Map<StatusUserDto>(status);

        return new OkServiceResult<UserDto>("profile", userDto);
    }

    public async Task<ServiceResultBase<bool>> Refresh()
    {

        return new OkServiceResult<bool>(true);
    }
}

