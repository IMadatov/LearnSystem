using BaseCrud.Entities;

namespace LearnSystem.Models.ModelsDTO;

public class SubjectDto:IDataTransferObject<Subject>
{
    public SubjectDto() { }

    public SubjectDto(SubjectDto subject, UserDto userDto)
    {
        Id = subject.Id;
        Name = subject.Name;
        CraetedDate = subject.CraetedDate;
        CreatedBy = subject.CreatedBy;
        User = userDto;
    }
    public int? Id { get; set; }

    public string? Name { get; set; }

    public DateTime? CraetedDate { get; set; } = DateTime.Now;

    public string? CreatedBy { get; set; }

    public UserDto? User { get; set; }
        
    public string Creator { get
        {
            return User!=null?User.UserName:"";
        } }
} 




