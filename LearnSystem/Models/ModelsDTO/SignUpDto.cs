namespace LearnSystem.Models.ModelsDTO
{
    public class SignUpDto
    {
        public string Email { get; set; }
        
        public string? Password { get; set; }
        
        public string FirstName {  get; set; }

        public string LastName { get; set; }

        public string UserName {  get; set; }

        public string Rememberme {  get; set; }
    }
}
