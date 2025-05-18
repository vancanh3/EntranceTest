using AuthenticationAPI.Infrastructure;
using AuthenticationAPI.Infrastructure.Entities;
using AuthenticationAPI.Repositories.Interfaces;
using Org.BouncyCastle.Asn1.Ocsp;

namespace AuthenticationAPI.Repositories.Implements
{
    public class UserRepository : IUserRepository
    {
        private readonly AuthenticationDbContext _context;

        public UserRepository(AuthenticationDbContext context)
        {
            _context = context;
        }

        public async Task AddUserAsync(User user)
        {
            await _context.AddAsync(user);
        }

        public User GetUserByEmail(string email)
        {
            return _context.Users.FirstOrDefault(x => x.Email == email);
        }

        public User GetUserById(int userId)
        {
            return _context.Users.FirstOrDefault(x => x.Id == userId);
        }

        public async Task<int> SaveDataToDbAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
