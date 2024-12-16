using LearnSystem.Models;
using Telegram.Bot;
using LearnSystem.Services.IServices;
using LearnSystem.DbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using ServiceStatusResult;
using System.Security.Claims;




namespace LearnSystem.Services;

public class TelegramService : ITelegramService
{
    private readonly IConfiguration configuration;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> userManager;
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly TelegramBotClient bot;


    public TelegramService(
    IConfiguration configuration,
    ApplicationDbContext _context,
    UserManager<User> userManager,
    IHttpContextAccessor httpContextAccessor
    )
    {
        this.configuration = configuration;
        this._context = _context;
        this.userManager = userManager;
        this.httpContextAccessor = httpContextAccessor;
        bot = new TelegramBotClient(configuration.GetValue<string>("Telegram:TokenBot"));

    }
    public async Task<string?> UserProfilePhotos(User user)
    {
        var userTelegramId = long.Parse(user.TelegramId);
        var chatId = new Telegram.Bot.Types.ChatId(userTelegramId);


        var chat = await bot.GetChatAsync(chatId);

        var userStatus = await _context.StatusUsers.AsNoTracking().FirstOrDefaultAsync(x => x.User.TelegramId == userTelegramId.ToString());



        if (userStatus == null)
        {
            userStatus = new StatusUser
            {
                HasPhotoProfile = true,
                IsOnTelegramBotActive = true
            };
            _context.StatusUsers.Add(userStatus);
        }

        var userFromDb = await _context.Users.FirstOrDefaultAsync(x => x.TelegramId == user.TelegramId);

        userFromDb.StatusUser.Id = userStatus.Id;

        await _context.SaveChangesAsync();

        if (chat == null)
        {

        }

        Telegram.Bot.Types.File fileImg = await bot.GetFileAsync(chat.Photo.BigFileId);

        string filePath = fileImg.FilePath;

        using var outputStream = new MemoryStream();

        var file = await bot.GetInfoAndDownloadFileAsync(chat.Photo.BigFileId, outputStream);

        var array = outputStream.ToArray();

        var ms = new MemoryStream(array);


        return user.TelegramUserName;
    }


    public async Task<ServiceResultBase<bool>> RefreshUserInfo()
    {

        var userId = httpContextAccessor.HttpContext!.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)!.Value;

        if (userId == null)
            return new UnauthorizedServiceResult<bool>(false);


        var user = await _context.Users.Include(x=>x.StatusUser).FirstOrDefaultAsync(x => x.Id == Guid.Parse(userId));

        if (user == null)
            return new NotFoundServiceResult<bool>("user not found from db");

        var userTelegramId = long.Parse(user.TelegramId);

        var chatId = new Telegram.Bot.Types.ChatId(userTelegramId);

        var statusUser = user.StatusUser;

        if (statusUser == null)
        {
            statusUser = new StatusUser
            {
                HasPhotoProfile = false,
                //IsActiveAccount = false,
                IsOnTelegramBotActive = false
            };

            _context.StatusUsers.Add(statusUser);

            user.StatusUser.Id = statusUser.Id;

            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        try
        {
            var chat = await bot.GetChatAsync(chatId);

            user.TelegramUserName = chat.Username;

            user.FirstName = chat.FirstName;

            user.LastName = chat.LastName;

            statusUser.IsOnTelegramBotActive = true;

            statusUser.HasPhotoProfile = true;
            if (chat.Photo == null)
            {
                statusUser.HasPhotoProfile = false;
            }
        }
        catch (Exception e)
        {
            statusUser.HasPhotoProfile = false;

            statusUser.IsOnTelegramBotActive = false;

            Console.WriteLine(e);
        }

        _context.Entry(statusUser).State = EntityState.Modified;

        await _context.SaveChangesAsync();

        return new OkServiceResult<bool>(true);
    }

    public async Task<bool> ChangeStatusUser(StatusUser statusUser)
    {

        if (statusUser == null)
        {
            return false;
        }
        _context.StatusUsers.Add(statusUser);

        await _context.SaveChangesAsync();


        return true;
    }

    public async Task<bool> SendMassage(long telegramId, string massage)
    {
        try
        {

            var chatId = new Telegram.Bot.Types.ChatId(telegramId);

            var massager = await bot.SendTextMessageAsync(chatId, massage);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        return false;


    }
}
