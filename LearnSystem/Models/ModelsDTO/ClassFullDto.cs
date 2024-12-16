using BaseCrud.Entities;

namespace LearnSystem.Models.ModelsDTO
{
    public class ClassFullDto:IDataTransferObject<Class>
    {
        public int Dagree { get; set; }

        public string Name { get; set; } = string.Empty;

    }
}
