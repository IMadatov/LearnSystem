using AutoMapper;
using BaseCrud.Abstractions.Entities;
using BaseCrud.Entities;
using BaseCrud.EntityFrameworkCore;
using BaseCrud.Errors;
using BaseCrud.Errors.Keys;
using BaseCrud.ServiceResults;
using LearnSystem.DbContext;
using LearnSystem.Models;
using LearnSystem.Models.ModelsDTO;
using LearnSystem.ServicesBaseCrud.IServiceBaseCrud;
using Microsoft.EntityFrameworkCore;

namespace LearnSystem.ServicesBaseCrud;

public class SubjectBaseCrudService(
    ApplicationDbContext dbContext,
    IMapper mapper
) : BaseCrudService<Subject, SubjectDto, SubjectDto, int, Guid>(dbContext, mapper),
    ISubjectBaseCrudService
{
    public override async Task<ServiceResult<QueryResult<SubjectDto>>> GetAllAsync(
        IDataTableMetaData dataTableMeta,
        IUserProfile<Guid>? userProfile,
        Func<CrudActionContext<Subject, int, Guid>, ValueTask<IQueryable<Subject>>>? customAction = null,
        CancellationToken cancellationToken = default
    )
    {
        if (dataTableMeta.PaginationMetaData.Rows <= 0)
            return BadRequest(new DataTableValidationServiceError(
                "Rows must be greater than 0",
                ErrorKey:
                ErrorKeys.Validation.Datatable.RowsCountMustBeGreaterThanZero
                ));

        if (dataTableMeta.PaginationMetaData.First < 0)
            return BadRequest(
                new DataTableValidationServiceError(
                    "First must be greater than or equal to 0",
                    ErrorKey: ErrorKeys.Validation.Datatable.FirstMustBeGreaterThanOrEqualToZero)
            );

        IQueryable<Subject> query = Set;

        query = HandleGlobalFilter(dataTableMeta, query);

        if (customAction != null)
            query = await customAction(
                new CrudActionContext<Subject, int, Guid>(
                    query,
                    userProfile,
                    Mapper, 
                    dataTableMeta, 
                    cancellationToken
                    )
            );

        int totalCount = await query.CountAsync(cancellationToken);

        List<SubjectDto> data = await RetrieveDataAsync(dataTableMeta, query, cancellationToken);

        ServiceResult<(int totalCount, IEnumerable<SubjectDto> data)> queryResult = (totalCount, data);

        if (!queryResult.IsSuccess)
            return ServiceResult.FromFailed(queryResult).ToType<QueryResult<SubjectDto>>();

        foreach(var item in data)
        {
            item.User =await  dbContext.Users.Where(u => u.Id == Guid.Parse(item.CreatedBy!)).Select(ud => 
            new UserDto { 
                FirstName = ud.FirstName!,
                LastName=ud.LastName!,
                Id=ud.Id,
                UserName=ud.UserName!
            }
            ).FirstOrDefaultAsync();
        }

        var result = new QueryResult<SubjectDto>
        {
            Items = data,
            TotalItems=totalCount
        };

        return result;
    }

    public async Task<string> GetUsernameBySubjectId(int subjectId)
    {
        var subjectWithUser = await dbContext.Subjects
            .Where(s => s.Id == subjectId)
            .Select(s => new { Subject = s, User = dbContext.Users.First(u => u.Id.ToString() == s.CreatedBy) })
            .FirstOrDefaultAsync();

        return subjectWithUser?.User.UserName ?? throw new InvalidOperationException();
    }
}

public class SubjectDtoWithUser : SubjectDto
{
    public string Username { get; set; }
}
