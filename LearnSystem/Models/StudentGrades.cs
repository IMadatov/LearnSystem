using BaseCrud.Abstractions.Entities;
using System.ComponentModel.DataAnnotations;

namespace LearnSystem.Models;

public class StudentGrades : EntityBase
{
    public int? StudentId { get; set; }

    public Student? Student { get; set; }

    public int LessonId { get; set; }

    public Lesson? Lesson { get; set; }

    [RegularExpression("^[+-]?(10|[0-9])$")]
    [Length(1, 2)]
    public string Score { get; set; } = string.Empty;
}
