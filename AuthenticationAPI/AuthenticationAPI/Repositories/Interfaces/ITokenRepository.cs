using AuthenticationAPI.Infrastructure.Entities;

namespace AuthenticationAPI.Repositories.Interfaces
{
    public interface ITokenRepository
    {
        Token GetByRefreshToken(string token);
        Task AddNewTokenAsync(Token token);
        void RemoveOldToken(Token oldToken);
        void RemoveListTokens(List<Token> removeTokens);
        Task SaveDataToDbAsync();
        Task<List<Token>> GetAllTokensByUserId(int userId);
    }
}
