namespace LearnSystem.Models;


public class StatusUser
{

    public string Id { get; set; }

    public bool IsOnTelegramBotActive { get; set; }

    public bool IsActiveAccount { get; set; }

    public bool hasPhotoProfile { get; set; }

    public User? User { get; set; }
}
