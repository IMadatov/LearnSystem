namespace LearnSystem.Models;

public class Student
{
    public int Id { get; set; }

    public User? User { get; set; }

    public int ClassId { get; set; }

    public Class? Class { get; set; }

    public ICollection<StudentGrades> Grades { get; set; } = new List<StudentGrades>();
}
