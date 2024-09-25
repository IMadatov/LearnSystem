using LearnSystem.DbContext;
using LearnSystem.Models;
using LearnSystem.Models.ModelsDTO;
using LearnSystem.Services;
using LearnSystem.Services.IServices;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication()
    .AddCookie(IdentityConstants.ApplicationScheme, opt =>
    {

        opt.ExpireTimeSpan = TimeSpan.FromHours(3);
        opt.Events.OnRedirectToLogin = (context) =>
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        };

        opt.Cookie.SameSite = SameSiteMode.None;
        //opt.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });

builder.Services.AddAuthorization(opt =>
{
    opt.AddPolicy("admin", policy => {
       
        policy.RequireRole("admin");
    });
});

builder.Services.AddIdentityCore<User>()
    .AddRoles<Roles>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddApiEndpoints();

builder.Services.Configure<IdentityOptions>(option =>
{
    option.User.RequireUniqueEmail = false;

    //User
    option.User.AllowedUserNameCharacters =
    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    //Lockout
    option.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
    option.Lockout.MaxFailedAccessAttempts = 10000;
    option.Lockout.AllowedForNewUsers = true;

    //Password

});


builder.Services.AddDbContext<ApplicationDbContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("dev"));
});

builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IAuthService, AuthService>();
builder.Services.AddTransient<IRoleService, RoleService>();


builder.Services.AddAutoMapper(opt =>
{
    opt.CreateMap<User, UserDto>().ReverseMap();

});


builder.Services.AddSwaggerGen();



builder.Services.AddCors(opt =>
{
    //opt.AddPolicy("AllowAll", buil => buil.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

    opt.AddPolicy("ToGlobal",
        buil => buil
            .WithOrigins("https://9d57-188-113-250-212.ngrok-free.app").AllowAnyHeader().AllowAnyMethod().AllowCredentials());
    opt.AddPolicy("ToLocal",
       buil => buil
           .WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod().AllowCredentials());

});














var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI();
    app.UseSwagger();
}
// Configure the HTTP request pipeline.

app.UseHttpsRedirection();



app.UseCors("ToLocal");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapIdentityApi<User>().AllowAnonymous();
app.Run();

