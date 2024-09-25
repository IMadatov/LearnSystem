namespace LearnSystem.Models;

public class Subject
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public ICollection<Journal> Journals { get; set; } = new List<Journal>();

    public ICollection<Teacher> Teachers { get; set; } = new List<Teacher>();
}
