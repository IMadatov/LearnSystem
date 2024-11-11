using BaseCrud.Abstractions.Entities;

namespace LearnSystem.Models;

public class Student : EntityBase
{
    public User? User { get; set; }

    public Guid UserId { get; set; }

    public int ClassId { get; set; }

    public Class? Class { get; set; }

    public ICollection<StudentGrades> Grades { get; set; } = new List<StudentGrades>();
}
