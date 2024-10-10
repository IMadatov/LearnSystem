using LearnSystem.Models.ModelsDTO;
using ServiceStatusResult;

namespace LearnSystem.Services.IServices
{
    public interface IUserService
    {
        Task<ServiceResultBase<PaginatedList<UserDto>>> GetAdmins(int first, int rows);
        Task<ServiceResultBase<PaginatedList<UserDto>>> GetStudents(int first, int rows);
        Task<ServiceResultBase<PaginatedList<UserDto>>> GetTeachers(int first, int rows);
        Task<ServiceResultBase<PaginatedList<UserDto>>> GetUsers(int first, int rows);
        Task<ServiceResultBase<UserDto>> Me(string userId);
        Task<ServiceResultBase<bool>> Refresh();
    }
}
