namespace LearnSystem.Models.ModelsDTO;

public class SubjectDto
{
    public SubjectDto() { }

    public SubjectDto(SubjectDto subject, UserDto userDto)
    {
        Id = subject.Id;
        Name = subject.Name;
        AtCreate = subject.AtCreate;
        UserId = subject.UserId;
        User = userDto;
    }
    public int? Id { get; set; }

    public string? Name { get; set; }

    public DateTime? AtCreate { get; set; } = DateTime.Now;

    public string? UserId { get; set; }

    public UserDto? User { get; set; }
}

