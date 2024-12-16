using BaseCrud.EntityFrameworkCore.Services;
using LearnSystem.Models;
using LearnSystem.Models.ModelsDTO;

namespace LearnSystem.ServicesBaseCrud.IServiceBaseCrud;
public interface IUserBaseCrudService : IEfCrudService<User, UserDto, UserDto, Guid, Guid>;