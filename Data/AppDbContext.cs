using LMS.Models;
using Microsoft.EntityFrameworkCore;

namespace LMS.Data
{
    public class AppDbContext : DbContext
    {
        // Constructor for dependency injection
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // DbSet for ApplicationUser table
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    }
}
