namespace LearnSystem.Models.ModelsDTO
{
    public class StatusUserDto
    {
        public string Id { get; set; }

        public bool IsOnTelegramBotActive { get; set; }

        public bool IsActiveAccount { get; set; }

        public bool hasPhotoProfile { get; set; }
    }
}