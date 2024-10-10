using LearnSystem;
using LearnSystem.DbContext;
using LearnSystem.Models;
using LearnSystem.Models.ModelsDTO;
using LearnSystem.Services;
using LearnSystem.Services.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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
        opt.Events.OnRedirectToAccessDenied = context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        };
        opt.Cookie.SameSite = SameSiteMode.None;
        opt.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });

builder.Services.AddAuthorization(opt =>
{
    opt.AddPolicy("Admin", policy =>
    {
        policy.RequireRole("admin");
    });
});




builder.Services.AddIdentityCore<User>()
    .AddRoles<Roles>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddClaimsPrincipalFactory<CustomUserClaimsPrincipalFactory>()
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
builder.Services.AddTransient<IAdminService, AdminService>();
builder.Services.AddTransient<ITelegramService, TelegramService>();
builder.Services.AddTransient<ITeacherService, TeacherService>();

builder.Services.AddAutoMapper(opt =>
{
    opt.CreateMap<User, UserDto>().ReverseMap();
    opt.CreateMap<StatusUser, StatusUserDto>().ReverseMap();
    opt.CreateMap<Subject, SubjectDto>().ReverseMap();
});


builder.Services.AddSwaggerGen();


builder.Services.AddCors(opt =>
{
    //opt.AddPolicy("AllowAll", buil => buil.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

    opt.AddPolicy("ToGlobal",
        buil => buil
            .WithOrigins("https://cgtlb6bz-4200.euw.devtunnels.ms").AllowAnyHeader().AllowAnyMethod().AllowCredentials());
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

