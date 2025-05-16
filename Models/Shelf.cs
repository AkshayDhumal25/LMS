namespace LMS.Models
{
    public class Shelf
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int SectionId { get; set; }
        public Section Section { get; set; }
        public List<Row> Rows { get; set; }
    }

}
