using AuthenticationAPI.Infrastructure.Entities;

namespace AuthenticationAPI.Repositories.Interfaces
{
    public interface IUserRepository
    {
        User GetUserByEmail(string email);
        User GetUserById(int id);
        Task AddUserAsync(User user);
        Task<int> SaveDataToDbAsync();
    }
}
