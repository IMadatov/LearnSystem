using LearnSystem.Models;
using LearnSystem.Models.ModelsDTO;
using ServiceStatusResult;

namespace LearnSystem.Services.IServices
{
    public interface IUserService
    {
        Task<ServiceResultBase<List<User>>> GetUsers();
        Task<ServiceResultBase<UserDto>> Me(string userId);
    }
}
