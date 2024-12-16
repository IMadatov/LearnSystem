using AutoMapper;
using BaseCrud.EntityFrameworkCore;
using LearnSystem.DbContext;
using LearnSystem.Models;
using LearnSystem.Models.ModelsDTO;
using LearnSystem.ServicesBaseCrud.IServiceBaseCrud;

namespace LearnSystem.ServicesBaseCrud;

public class StudentBaseCrudService(ApplicationDbContext dbContext, IMapper mapper) : BaseCrudService<Student, StudentDto, StudentFullDto,int,Guid>(dbContext, mapper), IStudentBaseCrudService
{
}
