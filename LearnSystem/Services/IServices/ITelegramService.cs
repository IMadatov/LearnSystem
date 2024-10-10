using LearnSystem.Models;
using ServiceStatusResult;

namespace LearnSystem.Services.IServices;

public interface ITelegramService
{
    Task<ServiceResultBase<bool>> RefreshUserInfo();
    public Task<string?> UserProfilePhotos(User user);
    Task<bool> SendMassage(long telegramId, string massage);
}
