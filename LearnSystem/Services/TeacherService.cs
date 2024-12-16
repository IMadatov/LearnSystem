using AutoMapper;
using LearnSystem.DbContext;
using LearnSystem.Models;
using LearnSystem.Models.ModelsDTO;
using LearnSystem.Services.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

        

        public async Task<ServiceResultBase<bool>> CreateClass(ClassDto classDto)
        {

            var UserId = httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
            var user = await userManager.FindByIdAsync(UserId);
            if (classDto == null) { return new BadRequesServiceResult<bool>("ClassDto is null", false); }

            if (classDto.Id != null)
            {
                return new BadRequesServiceResult<bool>("Id is not null", false);
            }

            var classObj = new Class
            {
                Name = classDto.Name,
                Dagree = (int)classDto.Dagree!,
            };



            context.Add(classObj);

            await context.SaveChangesAsync();

            var Teacher = await context.Teachers.FirstOrDefaultAsync(x => x.User == user);



            var journal = new Journal
            {
                ClassId = classObj.Id,
                Teachers = new List<Teacher> { Teacher },

            };

            context.Journales.Add(journal);

            await context.SaveChangesAsync();



            return new OkServiceResult<bool>(true);
        }

        public async Task<ServiceResultBase<bool>> DeleteClasses(List<int> ids)
        {
            var userId = httpContextAccessor.HttpContext!.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)!.Value;

            var user = await userManager.FindByIdAsync(userId);

            if (user == null) {
              return new UnauthorizedServiceResult<bool>(false);
            }

            var deleteClassL = context.Classes.Where(x =>ids.Contains(x.Id));

            var deleteClassList =await deleteClassL.Where(x => x.CreatedBy == user!.Id.ToString()).ToListAsync();

            context.Classes.RemoveRange(deleteClassList);

            await context.SaveChangesAsync();
            return new OkServiceResult<bool>(true);
        }

        public async Task<ServiceResultBase<bool>> CreateSubject(SubjectDto createSubjectDto)
        {

            var UserId = httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;

            createSubjectDto.CreatedBy = UserId;



            var user = await context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == Guid.Parse(UserId));

            if (user == null)
            {
                return new UnauthorizedServiceResult<bool>(false);
            }



            var teacher = await context.Teachers.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == user.Id);

            if (teacher == null)
            {
                teacher = new Teacher { UserId = user.Id };
                context.Teachers.Add(teacher);
                await context.SaveChangesAsync();
            }
            var subject = new Subject
            {
                Name = createSubjectDto.Name,
            };

            context.Add(subject);

            await context.SaveChangesAsync();

            
            return new OkServiceResult<bool>(true);
        }

        public async Task<ServiceResultBase<PaginatedList<SubjectDto>>> GetSubjects(int first, int row, string field, short order)
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
                .Select(subject => new SubjectDto(mapper.Map<SubjectDto>(subject), mapper.Map<UserDto>(
                    context.Users.AsNoTracking().FirstOrDefault(x => x.Id == Guid.Parse(subject.CreatedBy!))!)
                    ));

            var allSql = allSubjects.ToQueryString();



            var subjects = await allSubjects.Skip(first).Take(row).ToListAsync();

            var count = context.Subjects.Count();




            var result = new PaginatedList<SubjectDto>(mapper.Map<List<SubjectDto>>(subjects), count);

            return new OkServiceResult<PaginatedList<SubjectDto>>(result);
        }

        public async Task<ServiceResultBase<List<UserDto>>> GetAllStudents(int first, int row, string field, short order)
        {

            var studentsContext = context.Students.AsQueryable();
            var property = typeof(Student).GetProperty(field);
            if (property != null)
            {
                ParameterExpression p = Expression.Parameter(typeof(Student), "x");

                var propertyAccess = Expression.MakeMemberAccess(p, property);

                var lambda = Expression.Lambda<Func<Student, object>>(propertyAccess, p);

                if (order == -1)
                {
                    studentsContext = studentsContext.OrderByDescending(lambda);
                }
                else studentsContext = studentsContext.OrderBy(lambda);
            }
            var students = studentsContext.Skip(first).Take(row);

            var studentsDto = mapper.Map<List<UserDto>>(students);

            return studentsDto;
        }

        public async Task<ServiceResultBase<bool>> DeleteSubjects(List<SubjectDto> subjectDtos)
        {
            var userId = httpContextAccessor.HttpContext!.User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;


            var filteredMySubject = subjectDtos.Where(x => x.CreatedBy == userId);

            if (filteredMySubject.Count() == 0)
            {
                return new OkServiceResult<bool>(false);
            }
            context.Subjects.RemoveRange(mapper.Map<List<Subject>>(filteredMySubject));

            await context.SaveChangesAsync();

            return new OkServiceResult<bool>(true);
        }

        public async Task<ServiceResultBase<PaginatedList<ClassDto>>> GetAllClass(int first, int row, string field, short order)
        {
            var query = context.Classes.AsQueryable();

            var property = typeof(Class).GetProperty(field);

            if (property != null)
            {
                ParameterExpression p = Expression.Parameter(typeof(Class), "x");
                var propertyAccess = Expression.MakeMemberAccess(p, property);

                var lambda = Expression.Lambda<Func<Class, object>>(propertyAccess, p);

                if (order == -1) query = query.OrderByDescending(lambda);
                else query = query.OrderBy(lambda);
            }

            var classes = await query.Skip(first).Take(row).ToListAsync();

            var result = new PaginatedList<ClassDto>(mapper.Map<List<ClassDto>>(classes), context.Classes.Count());

            return new OkServiceResult<PaginatedList<ClassDto>>(result);
        }
    }
}
