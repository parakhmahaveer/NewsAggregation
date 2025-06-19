using NewsAggrigation.DAL.Models;

namespace NewsAggrigation.DAL.Repositories.Auth
{
    public interface IAuthRepository
    {
        Task<User?> GetUserByUsernameAsync(string username);
        Task AddUserAsync(User user);
        Task SaveChangesAsync();
        Task<User?> GetUserByEmailAsync(string email);
    }
}
