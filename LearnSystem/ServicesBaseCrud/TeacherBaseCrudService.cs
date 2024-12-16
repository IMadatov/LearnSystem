using AutoMapper;
using BaseCrud.Abstractions.Entities;
using BaseCrud.Entities;
using BaseCrud.EntityFrameworkCore;
using BaseCrud.ServiceResults;
using LearnSystem.DbContext;
using LearnSystem.Models;
using LearnSystem.Models.ModelsDTO;
using LearnSystem.ServicesBaseCrud.IServiceBaseCrud;

namespace LearnSystem.ServicesBaseCrud
{
    public class TeacherBaseCrudService(ApplicationDbContext dbContext, IMapper mapper) : BaseCrudService<Teacher, TeacherDto, TeacherDtoFull,int,Guid>(dbContext, mapper), ITeacherBaseCrudService
    {
        public override Task<ServiceResult<QueryResult<TeacherDto>>> GetAllAsync(IDataTableMetaData dataTableMeta, IUserProfile<Guid>? userProfile, Func<CrudActionContext<Teacher, int, Guid>, ValueTask<IQueryable<Teacher>>>? customAction = null, CancellationToken cancellationToken = default)
        {
            var dbContext = DbContext as ApplicationDbContext;



            return base.GetAllAsync(dataTableMeta, userProfile, customAction, cancellationToken);
        }
    }
}
