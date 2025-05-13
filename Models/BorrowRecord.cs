namespace LMS.Models
{
    public class BorrowRecord
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public ApplicationUser User { get; set; }

        public int BookId { get; set; }
        public Book Book { get; set; }

        public DateTime BorrowedAt { get; set; }
        public DateTime? ReturnedAt { get; set; }

        public bool IsReturned { get; set; }
    }

}
