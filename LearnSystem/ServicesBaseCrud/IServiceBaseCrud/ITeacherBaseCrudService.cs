using BaseCrud.EntityFrameworkCore.Services;
using LearnSystem.Models;
using LearnSystem.Models.ModelsDTO;

namespace LearnSystem.ServicesBaseCrud.IServiceBaseCrud;

public interface ITeacherBaseCrudService : IEfCrudService<Teacher, TeacherDto, TeacherDtoFull,int,Guid>;

