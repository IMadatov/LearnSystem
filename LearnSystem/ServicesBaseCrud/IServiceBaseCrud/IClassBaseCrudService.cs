using BaseCrud.EntityFrameworkCore.Services;
using LearnSystem.Models;
using LearnSystem.Models.ModelsDTO;

namespace LearnSystem.ServicesBaseCrud.IServiceBaseCrud;

public interface IClassBaseCrudService : IEfCrudService<Class, ClassDto, ClassFullDto,int,Guid>;

