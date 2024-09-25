namespace LearnSystem.Models.ModelsDTO
{
    public class SignInDto
    {
        public string Username { get; set; }   
        
        public string Password { get; set; }

        public bool RememberMe { get; set; } = false;

        
    }
}
