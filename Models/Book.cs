//namespace LMS.Models
//{
//    public class Book
//    {
//        public int Id { get; set; }
//        public string Title { get; set; }
//        public string Author { get; set; }
//        public string ISBN { get; set; }
//        public string Category { get; set; }
//        public int TotalCopies { get; set; }
//    }

//}


using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string ISBN { get; set; }
        public string Category { get; set; }
        public int TotalCopies { get; set; }

        // New: Row Relationship
        public int RowId { get; set; }  // Foreign key
        public Row Row { get; set; }    // Navigation property
    }
}
