namespace LMS.Models
{
    public class ApplicationUser
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }  // In production, always hash passwords
        public string Name { get; set; }
        public string Email { get; set; }
        public string ContactNumber { get; set; }

        // Binary logic: true for Admin, false for User
        public bool IsAdmin { get; set; }
    }
}
