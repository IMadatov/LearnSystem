using BaseCrud.Entities;

namespace LearnSystem.Models.ModelsDTO;

public class StudentDto : IDataTransferObject<Student>
{
    public User? User { get; set; }

    public Guid UserId { get; set; }

    public int ClassId { get; set; }

    public Class? Class { get; set; }

}