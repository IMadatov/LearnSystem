using AutoMapper;
using BaseCrud.Abstractions.Entities;
using BaseCrud.Entities;
using BaseCrud.EntityFrameworkCore;
using BaseCrud.Errors.Keys;
using BaseCrud.Errors;
using BaseCrud.ServiceResults;
using LearnSystem.DbContext;
using LearnSystem.Models;
using LearnSystem.Models.ModelsDTO;
using LearnSystem.ServicesBaseCrud.IServiceBaseCrud;
using Microsoft.EntityFrameworkCore;

namespace LearnSystem.ServicesBaseCrud;

public class ClassBaseCrudService(ApplicationDbContext dbContext, IMapper mapper) : BaseCrudService<Class, ClassDto, ClassFullDto,Guid>(dbContext, mapper), IClassBaseCrudService
{

    public override async Task<ServiceResult<QueryResult<ClassDto>>> GetAllAsync(IDataTableMetaData dataTableMeta, IUserProfile<Guid> userProfile, Func<CrudActionContext<Class, int, Guid>, ValueTask<IQueryable<Class>>>? customAction = null, CancellationToken cancellationToken = default)
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

        IQueryable<Class> query = Set;

        
        query = query.Where(c => c.CreatedBy == userProfile.Id.ToString());
        query = HandleGlobalFilter(dataTableMeta, query);

        if (customAction != null)
            query = await customAction(
                new CrudActionContext<Class, int, Guid>(
                    query,
                    userProfile,
                    Mapper,
                    dataTableMeta,
                    cancellationToken
                    )
            );

        int totalCount = await query.CountAsync(cancellationToken);

        List<ClassDto> data = await RetrieveDataAsync(dataTableMeta, query, cancellationToken);

        ServiceResult<(int totalCount, IEnumerable<ClassDto> data)> queryResult = (totalCount, data);

        if (!queryResult.IsSuccess)
            return ServiceResult.FromFailed(queryResult).ToType<QueryResult<ClassDto>>();

        data.ForEach( item =>
        {
            item.Creator =dbContext.Users.Where(u => u.Id == Guid.Parse(item.CreatedBy)).Select(u => u.UserName).FirstOrDefault();
        });


        var result = new QueryResult<ClassDto>
        {
            Items = data,
            TotalItems = totalCount
        };

        return result;

    }

    public override  async Task<ServiceResult<ClassFullDto>> InsertAsync(ClassFullDto dto, IUserProfile<Guid> userProfile, CancellationToken cancellationToken = default)
    {

        var userId = userProfile.Id;

        var teacher =await dbContext.Teachers.FirstOrDefaultAsync(t=>t.UserId==userId);

        if(teacher == null)            
            return BadRequest(new ServiceError("Teacher not found", ErrorKeys.Validation.Error));

        var mappedClass = Mapper.Map<Class>(dto);

        if (mappedClass == null)
            return BadRequest(new ServiceError("Mapping failed", ErrorKeys.Validation.Error));

        mappedClass.CreatedBy = userId.ToString();
        
        mappedClass.CreatedDate=DateTime.Now;

        mappedClass.Active = true;
        
        dbContext.Classes.Add(mappedClass);

        await dbContext.SaveChangesAsync(cancellationToken);


        


        var journal = new Journal
        {
            ClassId = mappedClass.Id,
            Teachers = new List<Teacher> { teacher },
            CreatedBy=userId.ToString(),
            CreatedDate=DateTime.Now
        };

        dbContext.Journales.Add(journal);

        await dbContext.SaveChangesAsync(cancellationToken);


        var dtoClass=mapper.Map<ClassFullDto>(mappedClass);

        return dtoClass;
    }
}
