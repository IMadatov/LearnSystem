using BaseCrud.EntityFrameworkCore.Services;
using LearnSystem.Models;
using LearnSystem.Models.ModelsDTO;

namespace LearnSystem.ServicesBaseCrud.IServiceBaseCrud;

public interface IStudentBaseCrudService : IEfCrudService<Student, StudentDto, StudentFullDto,int,Guid>;