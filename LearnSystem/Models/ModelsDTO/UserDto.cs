namespace LearnSystem.Models.ModelsDTO
{
    public class UserDto
    {

        public UserDto(){}

        public UserDto(UserDto userDto,StatusUserDto statusUserDto)
        {
            Status=statusUserDto;
            Id = userDto.Id;
            TelegramId = userDto.TelegramId;
            UserName = userDto.UserName;
            Email = userDto.Email;
            FirstName = userDto.FirstName;
            LastName = userDto.LastName;
            PhotoUrl = userDto.PhotoUrl;
            TelegramUserName = userDto.TelegramUserName;
            CreatedAt = userDto.CreatedAt;
        }

        public UserDto(IList<string> roles)
        {
            Roles = roles;
        }

        public UserDto(UserDto userDto, IList<string> roles)
        {
            this.Roles = roles;
            Id = userDto.Id;
            TelegramId = userDto.TelegramId;
            UserName = userDto.UserName;
            Email = userDto.Email;  
            FirstName = userDto.FirstName;
            LastName = userDto.LastName;
            PhotoUrl = userDto.PhotoUrl;
            TelegramUserName = userDto.TelegramUserName;
            CreatedAt = userDto.CreatedAt;
        }



        public string Id { get; set; }
        public string TelegramId { get; set; }
        public string UserName {  get;set; }
        public string Email {  get;set; }
        public string FirstName {  get;set; }
        public string LastName {  get;set; }
        public string PhotoUrl {  get;set; }
        public string TelegramUserName {  get;set; }
        public DateTime ?CreatedAt {  get;set; }


        public StatusUserDto? Status {  get;set; }

        public IList<string> Roles {  get;set; }
    }
}
