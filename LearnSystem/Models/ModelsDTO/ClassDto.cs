namespace LearnSystem.Models.ModelsDTO
{
    public class ClassDto
    {
        public int? Id { get; set; }

        public int? Dagree { get; set; }

        public string? Name { get; set; } = string.Empty;
        
        public UserDto? CreatorUser { get; set; }
    }
}
