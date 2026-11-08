using Microsoft.EntityFrameworkCore;
using UniversityCafeteria.Models;

namespace UniversityCafeteria.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(
            DbContextOptions<AppDbContext> options
        ) : base(options)
        {
        }

        public DbSet<Category> categories{ get; set; }

        public DbSet<Product> products { get; set; }

        public DbSet<Supplier> suppliers { get; set; }
    }
}