using BaseCrud.Entities;

namespace LearnSystem.Models.ModelsDTO
{
    public class ClassDto:IDataTransferObject<Class>
    {
        public int? Id { get; set; }

        public int? Dagree { get; set; }

        public string? Name { get; set; } = string.Empty;
        
        public string? Creator { get; set; }

        public string? CreatedBy { set; get; }
    }
}
