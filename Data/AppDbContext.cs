using Microsoft.EntityFrameworkCore;
using jQuery_Practice.Models;

namespace jQuery_Practice.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Class> Classes => Set<Class>();
    }
}
