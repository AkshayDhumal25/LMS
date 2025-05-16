namespace LMS.Models
{
    public class Row
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ShelfId { get; set; }
        public Shelf Shelf { get; set; }
        public List<Book> Books { get; set; }
    }

}
