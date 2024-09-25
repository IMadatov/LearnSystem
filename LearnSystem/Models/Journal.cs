namespace LearnSystem.Models;

public class Journal
{
    public int Id { get; set; }

    public int ClassId {  get; set; }
    public Class? Class { get; set; }

    public int SubjectId {  get; set; }
    public Subject? Subject { get; set; }

    public int TeacherId {  get; set; }
    public Teacher? Teacher { get; set; }

    public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
}
