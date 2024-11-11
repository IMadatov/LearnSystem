using BaseCrud.Abstractions.Entities;

namespace LearnSystem.Models;

public class StatusUser : EntityBase<Guid>
{
    public bool IsOnTelegramBotActive { get; set; }

    public bool HasPhotoProfile { get; set; }

    public User? User { get; set; }

    public Guid? UserId { get; set; }
}
