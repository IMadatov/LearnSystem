namespace LearnSystem.Models.ModelsDTO
{
    public class PaginatedList<T>
    {
        public List<T> Items { get; set; }
        public int AllCountItems { get; set; }

        public PaginatedList(List<T> Items,int AllCountItems) 
        { 
            this.Items = Items;
            this.AllCountItems = AllCountItems;
        }

    }
}
