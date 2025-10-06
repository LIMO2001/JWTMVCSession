using JwtSessionMvc.Models;

namespace JwtSessionMvc.Services
{
    public interface IUserService
    {
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> ValidateUserAsync(string email, string password);
        Task<User?> GetUserByIdAsync(int id);
        Task<User> AddUserAsync(User user);
        Task<User?> GetUserByUsernameAsync(string username); // optional
        void SetSomeSessionValue(string key, string value);
        string? GetSomeSessionValue(string key);
    }
}
