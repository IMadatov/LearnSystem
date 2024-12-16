using BaseCrud.Abstractions.Entities;

namespace LearnSystem.Models
{
    public class UserProfile : IUserProfile<Guid>
    {
        public string? UserName { get ; set ; }
        public string? Fullname { get ; set ; }
        public Guid Id { get ; set ; }
    }
}
