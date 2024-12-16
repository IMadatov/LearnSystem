using AutoMapper;
using BaseCrud.EntityFrameworkCore;
using LearnSystem.DbContext;
using LearnSystem.Models;
using LearnSystem.Models.ModelsDTO;
using LearnSystem.ServicesBaseCrud.IServiceBaseCrud;

namespace LearnSystem.ServicesBaseCrud;

public class JournalBaseCrudService(
    ApplicationDbContext context,
    IMapper mapper
    ):BaseCrudService<Journal,JournalDto,JournalDto,Guid>(context,mapper),
    IJournalBaseCrudService
{
        
}
