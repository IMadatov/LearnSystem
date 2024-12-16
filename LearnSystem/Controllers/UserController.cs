using BaseCrud.Abstractions.Entities;
using BaseCrud.PrimeNg;
using LearnSystem.DbContext;
using LearnSystem.Models;
using LearnSystem.Models.ModelsDTO;
using LearnSystem.Services;
using LearnSystem.Services.IServices;
using LearnSystem.ServicesBaseCrud.IServiceBaseCrud;
using LearnSystem.SignalR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ServiceStatusResult;
using System.Security.Claims;

namespace LearnSystem.Controllers
{
    
    [Authorize]
    public class UserController(
        IUserService userService,
        IHubContext<LearnSystemSignalRHub, ISignalHubClient> hubContext,
        ITeacherBaseCrudService teacherBaseCrudService,
        IStudentBaseCrudService studentBaseCrudService,
        IUserBaseCrudService userBaseCrudService,
        ILogger<UserController> logger,
        ApplicationDbContext dbContext
        ) : BaseController
    {

        [HttpGet]
        public async Task<ActionResult> Refresh() =>
            await FromServiceResultBaseAsync(userService.Refresh());

        [HttpGet]
        public async Task<ActionResult<UserDto?>> Me()
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            return await FromServiceResult(userBaseCrudService.GetByIdAsync(Guid.Parse(userId), UserProfile));
        }

        [HttpPost]
        public async Task<ActionResult> SignOut()
        {
            await HttpContext.SignOutAsync();
            return Ok(true);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<QueryResult<UserDto>?>> GetAllUsers(PrimeTableMetaData primeTableMetaData) =>
            await FromServiceResult(userBaseCrudService.GetAllAsync(primeTableMetaData, UserProfile));


        [HttpPost]
        [Authorize(Roles = "admin, teacher")]
        public async Task<ActionResult<QueryResult<TeacherDto>?>> GetTeachers(PrimeTableMetaData primeTableMetaData) =>
            await FromServiceResult(teacherBaseCrudService.GetAllAsync(primeTableMetaData,UserProfile));


        [HttpPost]
        public async Task<ActionResult<QueryResult<StudentDto>?>> GetStudents(PrimeTableMetaData primeTableMetaData) =>
            await FromServiceResult(studentBaseCrudService.GetAllAsync(primeTableMetaData, UserProfile, context =>
            {
                var queryable = context.Queryable.Where(s => dbContext.Users.Any(u => u.Id == s.UserId));

                return ValueTask.FromResult(queryable);
            }));

        [HttpPost]
        public async Task<ActionResult<QueryResult<UserDto>?>> GetTeacherUsers(PrimeTableMetaData primeTableMetaData)
             => await FromServiceResult(userBaseCrudService.GetAllAsync(primeTableMetaData, UserProfile, context =>
             {
                 var queryable = context.Queryable.Where(u => dbContext.Teachers.Any(t => t.UserId == u.Id && u.Active));

                 var queryString = queryable.ToQueryString();

                 logger.LogInformation(queryString);

                 return ValueTask.FromResult(queryable);
             }));

        [HttpPost]
        public async Task<ActionResult<QueryResult<UserDto>?>> GetAdmins(PrimeTableMetaData primeTableMetaData)
            => await FromServiceResult(userBaseCrudService.GetAllAsync(primeTableMetaData, UserProfile, context =>
            {
                var queryable = context.Queryable.Where(u => dbContext.UserRoles.Any(ur=>ur.UserId==u.Id &&  ur.RoleId==dbContext.Roles.FirstOrDefault(r=>r.NormalizedName=="ADMIN")!.Id));

                return ValueTask.FromResult(queryable);
            }));
        protected async Task<ActionResult> FromServiceResultBaseAsync<T>(Task<ServiceResultBase<T>> task)
        {
            var result = await task;
            if (result == null)
            {
                return NoContent();
            }
            var isOk = result.StatusCode < 400;
            if (isOk)
            {
                return StatusCode(result.StatusCode, result.Result);
            }
            return StatusCode(result.StatusCode, "Request failed");
        }
    }
}
