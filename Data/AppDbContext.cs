using Microsoft.EntityFrameworkCore;
using JwtSessionMvc.Models;

namespace JwtSessionMvc.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Example tables — replace/add your own entities
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
    }
}
