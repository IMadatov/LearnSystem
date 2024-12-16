using LearnSystem;
using LearnSystem.BackgroundServices;
using LearnSystem.DbContext;
using LearnSystem.Models;
using LearnSystem.Models.ModelsDTO;
using LearnSystem.Services;
using LearnSystem.Services.IServices;
using LearnSystem.SignalR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BaseCrud;
using BaseCrud.Abstractions;
using System.Reflection;
using BaseCrud.PrimeNg;
using LearnSystem.Extensions.Converters;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole();

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(o =>
{
    o.JsonSerializerOptions.Converters.Add(new FilterMetadataConverter());
    o.JsonSerializerOptions.Converters.Add(new PrimeTableMetaConverter());
    o.JsonSerializerOptions.Converters.Add(new GuidConverter());
});

builder.Services.AddSignalR();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHostedService<ServiceTimeService>();

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

builder.Services.AddAuthorization();




builder.Services.AddIdentityCore<User>()
    .AddRoles<ApplicationRole>()
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

builder.Services.AddSingleton<MySaveChangesInterceptor>();

builder.Services.AddDbContext<ApplicationDbContext>((sp,opt) =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("dev"))
    .AddInterceptors(sp.GetRequiredService<MySaveChangesInterceptor>());
});

builder.Services.AddTransient<IUserService,UserService>();
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
    opt.CreateMap<Class, ClassDto>().ReverseMap();
});


builder.Services.AddSwaggerGen(x =>
{
    x.AddSignalRSwaggerGen();
});


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

builder.Services.AddBaseCrudService(
    new BaseCrudServiceOptions
    {
        Assemblies = [Assembly.GetExecutingAssembly()]
    }
);


var app = builder.Build();


//app.Use(async (context, next) =>
//{
//    try { await next(); } 
//    catch (Exception e){
//        Console.WriteLine(e);
//        await context.Response.WriteAsJsonAsync(new
//        {
//            ErrorMessage=e.Message,
//            ErrorKey="server_error"
//        });
//    }
//});

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

app.MapHub<LearnSystemSignalRHub>("/signalR");

app.MapControllers();


app.Run();

