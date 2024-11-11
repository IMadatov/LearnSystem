using LearnSystem.Models;
using LearnSystem.Models.ModelsDTO;
using BaseCrud.EntityFrameworkCore.Services;
using ServiceStatusResult;


namespace LearnSystem.Services.IServices;

public interface IUserService : IEfCrudService<User, UserDto, UserDto, Guid, Guid>
{
    Task<ServiceResultBase<PaginatedList<UserDto>>> GetAdmins(int first, int rows);
    Task<ServiceResultBase<PaginatedList<UserDto>>> GetStudents(int first, int rows);
    Task<ServiceResultBase<PaginatedList<UserDto>>> GetTeachers(int first, int rows);
    Task<ServiceResultBase<PaginatedList<UserDto>>> GetUsers(int first, int rows);
    Task<ServiceResultBase<UserDto>> Me(string userId);
    Task<ServiceResultBase<bool>> Refresh();
}
