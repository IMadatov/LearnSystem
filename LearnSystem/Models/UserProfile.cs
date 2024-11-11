using BaseCrud.Abstractions.Entities;

namespace LearnSystem.Models
{
    public class UserProfile : IUserProfile
    {
        public int Id { get ; set ; }
        public string? UserName { get ; set ; }
        public string? Fullname { get ; set ; }
    }
}
