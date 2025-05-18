using AuthenticationAPI.Infrastructure.Entities;

namespace AuthenticationAPI.Common.Utility
{
    public interface IJwtUtil
    {
        Tuple<string, string> GenerateToken(User user);
    }
}
