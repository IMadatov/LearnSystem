using LearnSystem.Models.ModelsDTO;
using ServiceStatusResult;

namespace LearnSystem.Services.IServices
{
    public interface IAdminService
    {

        // Task<ServiceResultBase<List<UserDto>> GetUsers();
        Task<ServiceResultBase<List<UserDto>>> GetUsers();
        Task<ServiceResultBase<List<UserDto>>> GetTeachers();
        Task<ServiceResultBase<bool>> UpdateRoleUser(ChangeRoleUserDto changeRoleUserDto);
        Task<ServiceResultBase<List<UserDto>>> GetStudents();
        Task<ServiceResultBase<PaginatedList<UserDto>> >GetAdmins(int First,int Rows);
        Task<ServiceResultBase<bool>> DeleteUser(string userId);
        Task<ServiceResultBase<bool>> DeactivateUser(string userId);
    }
}
