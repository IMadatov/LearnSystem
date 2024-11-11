using BaseCrud.Abstractions.Entities;
using System.ComponentModel.DataAnnotations;

namespace LearnSystem.Models;

public class Lesson : EntityBase
{
    public DateTime? Date { get; set; }

    public int JournalId { get; set; }

    public Journal? Journal { get; set; }

    [MaxLength(100)]
    public string ClassWork { get; set; } = string.Empty;

    public string? HomeWork { get; set; }

    public ICollection<StudentGrades> StudentGrades { get; set; } = [];
}
