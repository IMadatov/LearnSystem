using AutoMapper;
using LearnSystem.DbContext;
using LearnSystem.Models;
using LearnSystem.Models.ModelsDTO;
using LearnSystem.Services.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ServiceStatusResult;
using System.Collections.Generic;

namespace LearnSystem.Services;

public class AdminService(
    UserManager<User> userManager,
    RoleManager<ApplicationRole> roleManager,
    IMapper autoMapper,
    ApplicationDbContext _context
    ) : IAdminService
{
    public Task<ServiceResultBase<bool>> DeactivateUser(string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResultBase<bool>> DeleteUser(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return new NotFoundServiceResult<bool>(false);
        }

        var result = await userManager.DeleteAsync(user);

        if (result.Succeeded)
            return new OkServiceResult<bool>(true);

        return new OkServiceResult<bool>("Any error when deleting", false);
    }

    public async Task<ServiceResultBase<PaginatedList<UserDto>>> GetAdmins(int First, int Rows)
    {
        var alladmins = from admin in _context.Users
                        join status in _context.StatusUsers on admin.StatusUser.Id equals status.Id
                        join ur in _context.UserRoles on admin.Id equals ur.UserId
                        join role in _context.Roles on ur.RoleId equals role.Id
                        where role.Name == "admin"
                        select admin;

        var count = alladmins.Count();

        var admins = await alladmins.Include(x => x.StatusUser).Skip(First)
            .Take(Rows).ToListAsync();


        var userDto = autoMapper.Map<List<UserDto>>(admins);

        userDto.ForEach(x =>
        {
            x.Roles = new List<string>();
            x.Roles.Add("admin");
        });


        var results = new PaginatedList<UserDto>(userDto, count);

        return new OkServiceResult<PaginatedList<UserDto>>(results);

    }

    public async Task<ServiceResultBase<List<UserDto>>> GetStudents()
    {
        var students = await userManager.GetUsersInRoleAsync("student");

        var userDto = autoMapper.Map<List<UserDto>>(students);

        userDto.ForEach(x =>
        {

            x.Roles = new List<string>();
            x.Roles.Add("student");
        });

        return new OkServiceResult<List<UserDto>>(userDto);
    }

    public async Task<ServiceResultBase<List<UserDto>>> GetTeachers()
    {
        var teachers = await userManager.GetUsersInRoleAsync("teacher");

        var userDto = autoMapper.Map<List<UserDto>>(teachers);
        
        userDto.ForEach(x =>
        {
            x.Roles = new List<string>();
            x.Roles.Add("teacher");
        });

        return new OkServiceResult<List<UserDto>>(userDto);
    }

    public async Task<ServiceResultBase<List<UserDto>>> GetUsers()
    {
        var users = await userManager.Users.ToListAsync();



        var userDto = autoMapper.Map<List<UserDto>>(users);

        for (var i = 0; i < users.Count; i++)
        {
            userDto.ElementAt(i).Roles = await userManager.GetRolesAsync(users[i]);
        }

        return new OkServiceResult<List<UserDto>>(userDto);
    }

    public async Task<ServiceResultBase<bool>> UpdateRoleUser(ChangeRoleUserDto roleUserDto)
    {
        var roleFromDb = await roleManager.GetRoleNameAsync(new ApplicationRole { Name = roleUserDto.Role });

        var user = await userManager.FindByIdAsync(roleUserDto.UserId);

        var roles = await userManager.GetRolesAsync(user);

        await userManager.RemoveFromRolesAsync(user, roles);

        var result = await userManager.AddToRoleAsync(user, roleFromDb);

        

        if (result.Succeeded)
        {
            await RegrularationRoleAsync(roles.FirstOrDefault()!, roleFromDb, user);


            var statusUser = await _context.StatusUsers.FirstOrDefaultAsync(x => x.Id == user.StatusUser.Id);

            //statusUser.IsActiveAccount = roleUserDto.AccountStatus;

            _context.Entry(statusUser).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return new OkServiceResult<bool>(true);
        }

        return new OkServiceResult<bool>(false);
    }

    private async  Task RegrularationRoleAsync(string oldrole,string role,User user)
    {

        switch (oldrole)
        {
            case"student":
                var student =await _context.Students.FirstOrDefaultAsync(x => x.User == user);
                if (student != null) _context.Students.Remove(student);
                    break;
            case "teacher":
                var teacher = await _context.Teachers.FirstOrDefaultAsync(x => x.User == user);
                if (teacher != null) _context.Teachers.Remove(teacher);
                break;
        }

        await _context.SaveChangesAsync();

        switch(role)
        {
            case "teacher":
                var teacher = new Teacher { User = user };
                _context.Add(teacher);
                break;   
        }

        await _context.SaveChangesAsync();

        return ;
    }

}
