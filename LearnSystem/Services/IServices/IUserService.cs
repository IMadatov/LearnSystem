using LearnSystem.Models.ModelsDTO;
using ServiceStatusResult;


namespace LearnSystem.Services.IServices
{

    public interface IUserService
    {
        Task<ServiceResultBase<UserDto>> Me(Guid userId);
        Task<ServiceResultBase<bool>> Refresh();
    }
}