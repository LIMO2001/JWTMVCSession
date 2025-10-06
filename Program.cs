using JwtSessionMvc.Data;
using JwtSessionMvc.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace JwtSessionMvc
{
    public static class HttpContextHelper
    {
        public static IHttpContextAccessor? HttpContextAccessor { get; set; }
        public static HttpContext? Current => HttpContextAccessor?.HttpContext;
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add MVC Controllers with Views
            builder.Services.AddControllersWithViews();

            // Session configuration
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            // MySQL Database configuration
            builder.Services.AddDbContext<AppDbContext>(opts =>
                opts.UseMySql(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    new MySqlServerVersion(new Version(8, 0, 34))
                )
            );

            // Bind Jwt settings from configuration
            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

            // Authentication configuration (Cookie-based)
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(opts =>
                {
                    opts.LoginPath = "/Account/Login";
                    opts.LogoutPath = "/Account/Logout";
                    opts.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                });

            // Dependency Injection
            builder.Services.AddScoped<IJwtService, JwtService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddHttpContextAccessor();

            var app = builder.Build();

            // Set HttpContextAccessor for static helper
            HttpContextHelper.HttpContextAccessor = app.Services.GetRequiredService<IHttpContextAccessor>();

            // Middleware pipeline
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();

            // Default route
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }

    // 🔹 JwtSettings class used for configuration binding
    public class JwtSettings
    {
        public string Key { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int ExpiresMinutes { get; set; }
    }
}
