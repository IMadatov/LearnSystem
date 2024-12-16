using BaseCrud.EntityFrameworkCore.Services;
using LearnSystem.Models;
using LearnSystem.Models.ModelsDTO;

namespace LearnSystem.ServicesBaseCrud.IServiceBaseCrud;

public interface ISubjectBaseCrudService : IEfCrudService<Subject, SubjectDto, SubjectDto, int, Guid>;

