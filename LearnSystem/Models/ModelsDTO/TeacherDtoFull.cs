using BaseCrud.Entities;

namespace LearnSystem.Models.ModelsDTO;

public class TeacherDtoFull:IDataTransferObject<Teacher>
{
    public Guid UserId { get; set; }
    public User? User { get; set; }

    public ICollection<Journal> Journals { get; set; } = new List<Journal>();

    public ICollection<Subject> Subjects { get; set; } = new List<Subject>();
}
