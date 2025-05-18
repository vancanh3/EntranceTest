using AuthenticationAPI.Infrastructure;
using AuthenticationAPI.Infrastructure.Entities;
using AuthenticationAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationAPI.Repositories.Implements
{
    public class TokenRepository : ITokenRepository
    {
        private readonly AuthenticationDbContext _context;

        public TokenRepository(AuthenticationDbContext context)
        {
            _context = context;
        }

        public async Task AddNewTokenAsync(Token token)
        {
            await _context.Tokens.AddAsync(token);
        }

        public async Task<List<Token>> GetAllTokensByUserId(int userId)
        {
            return await _context.Tokens.Where(x => x.UserId == userId).ToListAsync();
        }

        public Token GetByRefreshToken(string token)
        {
            return _context.Tokens.FirstOrDefault(x => x.RefreshToken == token);
        }

        public void RemoveListTokens(List<Token> removeTokens)
        {
            _context.Tokens.RemoveRange(removeTokens);
        }

        public void RemoveOldToken(Token oldToken)
        {
            _context.Tokens.Remove(oldToken);
        }

        public async Task SaveDataToDbAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
