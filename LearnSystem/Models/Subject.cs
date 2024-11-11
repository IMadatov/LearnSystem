using BaseCrud.Abstractions.Entities;

namespace LearnSystem.Models;

public class Subject : EntityBase
{

    public string? Name { get; set; }

    public DateTime AtCreate { get; set; } = DateTime.Now;

    public User? User { get; set; }

    public Guid? UserId { get; set; }

    public ICollection<Journal> Journals { get; set; } = new List<Journal>();

    public ICollection<Teacher> Teachers { get; set; } = new List<Teacher>();
}
