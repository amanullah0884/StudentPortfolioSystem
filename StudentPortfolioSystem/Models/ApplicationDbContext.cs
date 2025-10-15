using Microsoft.EntityFrameworkCore;

namespace StudentPortfolioSystem.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)

       {

       }
        public DbSet<Student> Students { get; set; } = null!;
        public DbSet<Department> Departments { get; set; } = null!;
    }
}
