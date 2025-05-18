using AuthenticationAPI.Infrastructure.Entities;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using AuthenticationAPI.Services.Data.Request;
using System.Security.Cryptography;
using AuthenticationAPI.Common.Model;

namespace AuthenticationAPI.Common.Utility
{
    public class JwtUtil : IJwtUtil
    {
        private readonly AppSettings _appSettings;

        public JwtUtil(AppSettings appSettings)
        {
             _appSettings = appSettings;
        }

        public Tuple<string, string> GenerateToken(User user)
        {
            var handler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.SecretKey);
            var credentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature);

            var refreshToken = GenereateRefreshToken();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = GenerateClaims(user),
                Expires = DateTime.UtcNow.AddMinutes(60),
                SigningCredentials = credentials,
            };

            var token = handler.CreateToken(tokenDescriptor);
            var accessToken = handler.WriteToken(token);

            return new Tuple<string, string>(accessToken, refreshToken);
        }

        private string GenereateRefreshToken()
        {
            var random = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(random);
                return Convert.ToBase64String(random);
            }
        }

        private ClaimsIdentity GenerateClaims(User user)
        {
            var claims = new ClaimsIdentity();
            claims.AddClaim(new Claim("Email", user.Email));
            claims.AddClaim(new Claim("UserId", user.Id.ToString()));
            return claims;
        }
    }
}
