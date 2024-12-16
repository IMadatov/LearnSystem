using BaseCrud.EntityFrameworkCore.Services;
using LearnSystem.Models;
using LearnSystem.Models.ModelsDTO;

namespace LearnSystem.ServicesBaseCrud.IServiceBaseCrud;
public interface IJournalBaseCrudService:IEfCrudService<Journal,JournalDto,JournalDto,Guid>;
