using AutoMapper;
using LearnSystem.DbContext;
using LearnSystem.Models;
using LearnSystem.Models.ModelsDTO;
using LearnSystem.Services.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ServiceStatusResult;
using System.Linq.Expressions;
using System.Security.Claims;

namespace LearnSystem.Services
{
    public class TeacherService(
        ApplicationDbContext context,
        UserManager<User> userManager,
        IHttpContextAccessor httpContextAccessor,
        IMapper mapper
        ) : ITeacherService
    {
        public Task<ServiceResultBase<object>> CreateClass()
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResultBase<bool>> CreateSubject(SubjectDto createSubjectDto)
        {

            var UserId = httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;

            createSubjectDto.UserId=UserId;

            var subject = new Subject { 
                Name = createSubjectDto.Name,
                UserId=UserId,
            };
            context.Add(subject);

            await context.SaveChangesAsync();

            return new OkServiceResult<bool>(true);
        }

        public async Task<ServiceResultBase<PaginatedList<SubjectDto>>> GetSubjects(int first,int row,string field,short order)
        {
            var query = context.Subjects.AsQueryable();
            var property = typeof(Subject).GetProperty(field);

            if (property != null)
            {
                ParameterExpression p = Expression.Parameter(typeof(Subject), "x");
                
                var propertyAccess = Expression.MakeMemberAccess(p, property);

                var lambda = Expression.Lambda<Func<Subject, object>>(propertyAccess, p);
                
                //var orderByNullLambda = Expression.Lambda(
                //      Expression.Equal(propertyAccess, Expression.Constant(null)),
                //      p);

                //var resultExp = Expression.Call(
                //     typeof(Queryable),
                //     order == 1 ? "OrderBy" : "OrderByDescending",
                //     new[] { typeof(Subject), typeof(bool) },
                //     query.Expression,
                //     Expression.Quote(orderByNullLambda));



                //query = query.Provider.CreateQuery<Subject>(resultExp);
                if (order == -1)
                    query = query.OrderByDescending(lambda);
                else query = query.OrderBy(lambda);
                var sql = query.ToQueryString();
            }

            var allSubjects = query
                .Select(subject => new SubjectDto(mapper.Map<SubjectDto>(subject), mapper.Map<UserDto>(subject.User)));

            var allSql = allSubjects.ToQueryString();



            var subjects =await allSubjects.Skip(first).Take(row).ToListAsync();

            var count = context.Subjects.Count();




            var result = new PaginatedList<SubjectDto>(mapper.Map<List<SubjectDto>>(subjects), count);
           
            return new OkServiceResult<PaginatedList<SubjectDto>>(result);
        }

        public async Task<ServiceResultBase<List<UserDto>>> GetAllStudents()
        {
            var students = userManager.GetUsersInRoleAsync("teacher");

            var studentsDto = mapper.Map<List<UserDto>>(students);

            return studentsDto;
        }

        public async Task<ServiceResultBase<bool>> DeleteSubjects(List<SubjectDto> subjectDtos)
        {
            var userId = httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;


            var filteredMySubject = subjectDtos.Where(x => x.UserId == userId);
            if (filteredMySubject.Count()== 0)
            {
                return new OkServiceResult<bool>(false);
            }
            context.Subjects.RemoveRange(mapper.Map<List<Subject>>(filteredMySubject));

            await context.SaveChangesAsync();

            return new OkServiceResult<bool>(true);
        }
    }
}
