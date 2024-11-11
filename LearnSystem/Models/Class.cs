using BaseCrud.Abstractions.Entities;

namespace LearnSystem.Models;

public class Class : EntityBase
{

    public int Dagree { get; set; }

    public ICollection<Student> Students { get; set; } = new List<Student>();

    public string Name { get; set; } = string.Empty;

    public ICollection<Journal> Journals { get; set; } = [];
}
