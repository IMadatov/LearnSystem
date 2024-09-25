namespace LearnSystem.Models.ModelsDTO
{
    public class UserDto
    {
        public string Id { get; set; }
        public string TelegramId { get; set; }
        public string UserName {  get;set; }
        public string Email {  get;set; }
        public string FirstName {  get;set; }
        public string LastName {  get;set; }
        public string PhotoUrl {  get;set; }
        public string TelegramUserName {  get;set; }
        public DateTime ?CreatedAt {  get;set; }

        public IList<string> Roles {  get;set; }
    }
}
