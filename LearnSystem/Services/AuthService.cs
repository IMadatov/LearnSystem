using LearnSystem.DbContext;
using LearnSystem.Models;
using LearnSystem.Models.ModelsDTO;
using LearnSystem.Services.IServices;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ServiceStatusResult;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace LearnSystem.Services;

public class AuthService(
    ApplicationDbContext _dbContext,
    UserManager<User> userManager,
    IHttpContextAccessor httpContextAccessor,
    SignInManager<User> signInManager,
    IConfiguration configuration,
    ITelegramService telegramService,
    RoleManager<ApplicationRole> roleManager
    ) : IAuthService
{
    public async Task<ServiceResultBase<bool>> SignUpAsync(SignUpDto signUpDto)
    {
        var user = new User
        {
            Email = signUpDto.Email,
            FirstName = signUpDto.FirstName,
            LastName = signUpDto.LastName,
            UserName = signUpDto.UserName,
            CreatedDate = DateTime.Now
        };

        var result = await userManager.CreateAsync(user, signUpDto.Password!);

        if (result.Succeeded)
            return new OkServiceResult<bool>(true);

        return new BadRequesServiceResult<bool>(
            string.Join(
                Environment.NewLine,
                result.Errors.Select(x => x.Description)
            ),
            false);
    }

    public async Task<ServiceResultBase<bool>> SignOutAsync()
    {
        await signInManager.SignOutAsync();
        return new OkServiceResult<bool>(true);
    }

    public async Task<ServiceResultBase<bool>> SignInAsync(SignInDto signInDto)
    {
        var result = await signInManager.PasswordSignInAsync(signInDto.Username, signInDto.Password, signInDto.RememberMe, lockoutOnFailure: true);

        if (result.Succeeded)
        {

            return new OkServiceResult<bool>(true);
        }
        return new BadRequesServiceResult<bool>(false);
    }



    public async Task<ServiceResultBase<string>> OnSite()
    {
        var userId = httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;



        if (userId == null)
        {
            return new OkServiceResult<string>("false");
        }

        var user = await userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return new OkServiceResult<string>("false");
        }


        var role = await userManager.GetRolesAsync(user);

        return new OkServiceResult<string>(role.FirstOrDefault().ToString());
    }


    public async Task<ServiceResultBase<bool>> SignInWithTelegram(UserTelegram userTelegram)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.TelegramId == userTelegram.id);

        await signInManager.SignInAsync(user, true);

        //var result = await signInManager.SignInAsync()
        return new OkServiceResult<bool>(true);
    }


    public async Task<ServiceResultBase<bool>> SignUpWithTelegram(UserTeleramDTO userTeleramDTO, string telegramData)
    {

        var dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(telegramData);

        var userTelegram = Newtonsoft.Json.JsonConvert.DeserializeObject<UserTelegram>(telegramData);

        if (!await CheckAuthorizeFromBot(dictionary))
            return new BadRequesServiceResult<bool>("User is not authorize from bot still");

        var register = await userManager.Users.FirstOrDefaultAsync(x => x.TelegramId == userTelegram.id);

        if (register != null)
            return new BadRequesServiceResult<bool>("this user is registered", false);

        if (!await telegramService.SendMassage(long.Parse(userTelegram.id), "Hi. User of LMS. I\'m bot for learning system. You should not block the bot. If you block the bot, you won't be able to log in  "))
        {
            return new OkServiceResult<bool>("You don't acceptad", false);
        }


        var status = new StatusUser
        {
            HasPhotoProfile = false,
            IsOnTelegramBotActive = true
        };

        _dbContext.StatusUsers.Add(status);



        var user = new User
        {
            UserName = userTeleramDTO.UserName,
            Email = userTeleramDTO.Email,
            FirstName = userTelegram.first_name,
            LastName = userTelegram.last_name,
            AuthDate = userTelegram.auth_date,
            TelegramId = userTelegram.id,
            Hash = userTelegram.hash,
            TelegramUserName = userTelegram.username,
            PhotoUrl = userTelegram.photo_url,
            StatusUser = status,
            CreatedDate = DateTime.Now
        };





        var result = await userManager.CreateAsync(user, userTeleramDTO.Password);
        var countUser = _dbContext.Users.Count();
        if (result.Succeeded)
        {



            if (countUser < 4)
            {
                var rolesFromDb = await _dbContext.Roles.ToListAsync();


                if (rolesFromDb.Count == 0)
                {
                    var roles = new List<ApplicationRole> {
                        new ApplicationRole { Name="admin" },
                        new ApplicationRole { Name = "student" },
                        new ApplicationRole { Name = "teacher" }
                    };

                    foreach (var role in roles)
                    {
                        await roleManager.CreateAsync(role);
                    }
                }





                await userManager.AddToRoleAsync(user, "admin");
            }

            else
                await userManager.AddToRoleAsync(user, "student");

            await signInManager.SignInAsync(user, true);
            //var token = await userManager.CreateSecurityTokenAsync(user);

            //var loginInfoUser = new UserLoginInfo("Telegram",user.Hash, "");

            //var res = await userManager.AddLoginAsync(user, loginInfoUser);

            return new OkServiceResult<bool>(true);
        }



        return new BadRequesServiceResult<bool>(string.Join(Environment.NewLine, result.Errors.Select(x => x.Description)), false);

    }

    public async Task<ServiceResultBase<bool>> CheckTelegramData(string telegramData)
    {
        var dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(telegramData);
        if (!await CheckAuthorizeFromBot(dictionary))
            return new OkServiceResult<bool>(false);

        UserTelegram userTelegram = Newtonsoft.Json.JsonConvert.DeserializeObject<UserTelegram>(telegramData);

        var user = await userManager.Users.FirstOrDefaultAsync(x => x.TelegramId == userTelegram!.id);

        if (user == null)
            return new UnauthorizedServiceResult<bool>("You should be SignUp", false);

        await signInManager.SignInAsync(user, true);



        return new OkServiceResult<bool>(true);
    }

    private async Task<bool> CheckAuthorizeFromBot(Dictionary<string, string> userTelegram)
    {

        var botToken = configuration["Telegram:TokenBot"];

        using var sha256 = SHA256.Create();

        var secret = sha256.ComputeHash(Encoding.UTF8.GetBytes(botToken));

        var array = userTelegram
            .Where(k => k.Key != "hash")
            .Select(k => $"{k.Key}={k.Value}")
            .ToList();

        array.Sort();

        var sortedData = string.Join("\n", array);

        using var hmac = new HMACSHA256(secret);

        var checkHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(sortedData));

        var checkHashHex = BitConverter.ToString(checkHash).Replace("-", "").ToLower();

        return checkHashHex == userTelegram["hash"];

    }

    public async Task<ServiceResultBase<bool>> CheckUsername(string username)
    {
        var result = await userManager.FindByNameAsync(username);

        if (result == null)
            return new OkServiceResult<bool>(false);

        return new OkServiceResult<bool>(true);
    }

    public async Task<ServiceResultBase<Guid>> CreateRole(string roleName)
    {

        var role = new ApplicationRole
        {
            Name = roleName
        };
        await roleManager.CreateAsync(role);

        return new OkServiceResult<Guid>(role.Id);
    }
}


