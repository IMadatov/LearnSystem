namespace LearnSystem.Models;

public class Teacher
{
    public int Id { get; set; }

    public required User User { get; set; }

    public ICollection<Journal> Journals { get; set; } = new List<Journal>();

    public ICollection<Subject> Subjects { get; set; } = new List<Subject>();
}
