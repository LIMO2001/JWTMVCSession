using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace JwtSessionMvc.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            optionsBuilder.UseMySql(
                "server=localhost;database=jwt_session_mvc;user=root;password=Limo1125@;",
                new MySqlServerVersion(new Version(8, 0, 34))
            );

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
