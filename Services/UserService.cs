using JwtSessionMvc.Data;
using JwtSessionMvc.Models;
using Microsoft.EntityFrameworkCore;

namespace JwtSessionMvc.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        // ✅ Get user by Email (since login uses email)
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        // ✅ Validate user by Email + Password
        public async Task<User?> ValidateUserAsync(string email, string password)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email && u.PasswordHash == password);
            return user;
        }

        // ✅ Add a new user (Registration)
        public async Task<User> AddUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        // ✅ Get user by Id
        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        // ✅ Get user by Username (optional, if still used somewhere)
        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        // ✅ Session helper methods
        public void SetSomeSessionValue(string key, string value)
        {
            HttpContextHelper.Current?.Session.SetString(key, value);
        }

        public string? GetSomeSessionValue(string key)
        {
            return HttpContextHelper.Current?.Session.GetString(key);
        }
    }
}
