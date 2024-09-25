using LearnSystem.Models.ModelsDTO;
using ServiceStatusResult;

namespace LearnSystem.Services.IServices
{
    public interface IAuthService
    {
        Task<ServiceResultBase<bool>> SignUpAsync(SignUpDto signUpDto);

        Task<ServiceResultBase<bool>> SignOutAsync();

        Task<ServiceResultBase<bool>> SignInAsync(SignInDto signInDto);

        Task<ServiceResultBase<bool>> OnSite();

        Task<ServiceResultBase<bool>> SignUpWithTelegram(UserTeleramDTO userTeleramDTO,string telegramData);
        
        Task<ServiceResultBase<bool>> SignInWithTelegram(UserTelegram userTelegram);
        
        Task<ServiceResultBase<bool>> CheckUsername(string username);

        Task<ServiceResultBase<bool>> CheckTelegramData(string telegramData);
    }
}
