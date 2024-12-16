using BaseCrud.Entities;

namespace LearnSystem.Models.ModelsDTO
{
    public class StatusUserDto : IDataTransferObject<StatusUser, Guid>
    {
        public string Id { get; set; } = string.Empty;

        public bool IsOnTelegramBotActive { get; set; }

        public bool HasPhotoProfile { get; set; }
    }
}