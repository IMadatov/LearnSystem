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
using Microsoft.AspNetCore.Identity;

namespace LearnSystem.ServicesBaseCrud;

public class UserBaseCrudService(
    ApplicationDbContext dbContext,
    IMapper mapper,
    UserManager<User> userManager
)
    : BaseCrudService<User, UserDto, UserDto, Guid, Guid>(dbContext, mapper),
    IUserBaseCrudService
{
    public override async Task<ServiceResult<QueryResult<UserDto>>> GetAllAsync(
        IDataTableMetaData dataTableMeta,
        IUserProfile<Guid>? userProfile,
        Func<CrudActionContext<User, Guid, Guid>, ValueTask<IQueryable<User>>>? customAction = null, 
        CancellationToken cancellationToken = default)
    {
           if (dataTableMeta.PaginationMetaData.Rows <= 0)
            return BadRequest(
                new DataTableValidationServiceError(
                    "Rows must be greater than 0",
                    ErrorKey: ErrorKeys.Validation.Datatable.RowsCountMustBeGreaterThanZero)
            );

        if (dataTableMeta.PaginationMetaData.First < 0)
            return BadRequest(
                new DataTableValidationServiceError(
                    "First must be greater than or equal to 0",
                    ErrorKey: ErrorKeys.Validation.Datatable.FirstMustBeGreaterThanOrEqualToZero)
            );

        IQueryable<User> query = Set;

        query = HandleGlobalFilter(dataTableMeta, query);
        
        if (customAction != null)
            query = await customAction(
                new CrudActionContext<User, Guid, Guid>(
                    query,
                    userProfile,
                    Mapper,
                    dataTableMeta,
                    cancellationToken
                )
            );

        int totalCount = await query.CountAsync(cancellationToken);

        List<UserDto> data = await RetrieveDataAsync(dataTableMeta, query, cancellationToken);

        ServiceResult<(int totalCount, IEnumerable<UserDto> data)> queryResult = (totalCount, data);

        if (!queryResult.IsSuccess)
            return ServiceResult.FromFailed(queryResult).ToType<QueryResult<UserDto>>();

        var appDbContext = (DbContext as ApplicationDbContext)!;

        foreach (UserDto datum in data)
        {
            var guid = datum.Id;
            datum.Roles = await appDbContext.UserRoles
                .Where(r => r.UserId == guid)
                .Select(ur => appDbContext.Roles.First(r => r.Id == ur.RoleId).Name!)
                .ToListAsync(cancellationToken);
        }

        var result = new QueryResult<UserDto>
        {
            TotalItems = totalCount,
            Items = data
        };

        return result;
    }

    public override async Task<ServiceResult<UserDto?>> GetByIdAsync(Guid id, IUserProfile<Guid>? userProfile, Func<CrudActionContext<User, Guid, Guid>, ValueTask<IQueryable<User>>>? customAction = null, CancellationToken cancellationToken = default)
    {
         IQueryable<User> query =   Set;

        if (customAction != null)
            query = await customAction(
                new CrudActionContext<User, Guid, Guid>(
                    query,
                    userProfile,
                    Mapper,
                    null,
                    cancellationToken
                )
            );

        User user = await query.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        var roles =await (DbContext as ApplicationDbContext).Roles.Where(r => r.Id == (DbContext as ApplicationDbContext).UserRoles.FirstOrDefault(ur => ur.UserId == user.Id).RoleId).Select(r=>r.Name).ToListAsync();

        
        if ( user == null)
        {
            return NotFound(new NotFoundServiceError());
        }

        var result = Mapper.Map<UserDto>(user);

        result.Roles = roles;
        return result;
    }

}
