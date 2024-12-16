using AutoMapper;
using BaseCrud.Entities;

namespace LearnSystem.Models.ModelsDTO
{
    public class UserDto : IDataTransferObject<User, Guid>
    {
        //public UserDto SetData(UserDto userDto, StatusUserDto statusUserDto)
        //{
        //    Status = statusUserDto;
        //    Id = userDto.Id;
        //    TelegramId = userDto.TelegramId;
        //    UserName = userDto.UserName;
        //    Email = userDto.Email;
        //    FirstName = userDto.FirstName;
        //    LastName = userDto.LastName;
        //    PhotoUrl = userDto.PhotoUrl;
        //    TelegramUserName = userDto.TelegramUserName;
        //    CreatedAt = userDto.CreatedAt;
        //    Active = userDto.Active;

        //    return this;
        //}

        public Guid Id { get; set; }
        public string TelegramId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhotoUrl { get; set; }
        public string TelegramUserName { get; set; }
        public DateTime? CreatedDate { get; set; }

        public bool Active { get; set; }

        public StatusUserDto? StatusUser { get; set; }

        public IList<string> Roles { get; set; } = [];
    }
}
