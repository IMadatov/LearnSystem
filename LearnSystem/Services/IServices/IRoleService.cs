using LearnSystem.Models;
using ServiceStatusResult;

namespace LearnSystem.Services.IServices;

public interface IRoleService
{
    Task<ServiceResultBase<bool>> AddRole(string roleName);
    Task<ServiceResultBase<bool>> AddUserToRole(Models.ModelsDTO.ToRoleDto toRoleDto);
    Task<ServiceResultBase<List<ApplicationRole>>> GetAllRoles();
    public Task<ServiceResultBase<IList<string>>> MyRoles(string id);
}
