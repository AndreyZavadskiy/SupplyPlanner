namespace SP.Service.Models
{
    public class DictionaryListItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Active { get; set; }
    }

    public class DictionaryListItem<T>
    {
        public T Id { get; set; }
        public string Name { get; set; }
    }
}
