using BaseCrud.Entities;

namespace LearnSystem.Models.ModelsDTO;

public class TeacherDto:IDataTransferObject<Teacher>
{
    public UserDto? User { get; set; }
}
