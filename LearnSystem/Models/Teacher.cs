using BaseCrud.Abstractions.Entities;

namespace LearnSystem.Models;

public class Teacher : EntityBase
{
    public User? User { get; set; }

    public Guid UserId { get; set; }

    public ICollection<Journal> Journals { get; set; } = new List<Journal>();

    public ICollection<Subject> Subjects { get; set; } = new List<Subject>();
}
