using AuthenticationAPI.Common.Model;
using AuthenticationAPI.Common.Utility;
using AuthenticationAPI.Infrastructure.Entities;
using Microsoft.Extensions.Options;
using Xunit;

namespace AuthenticationAPI.Tests
{
    public class JwtUtilTests
    {
        private readonly JwtUtil _jwtUtil;
        private readonly AppSettings _appSettings;

        public JwtUtilTests()
        {
            _appSettings = new AppSettings
            {
                SecretKey = "your-test-secret-key-with-minimum-length-for-testing"
            };
            _jwtUtil = new JwtUtil(_appSettings);
        }

        [Fact]
        public void GenerateToken_WithValidUser_ReturnsTokens()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Email = "test@example.com",
                FirstName = "Test",
                LastName = "User"
            };

            // Act
            var (token, refreshToken) = _jwtUtil.GenerateToken(user);

            // Assert
            Assert.NotNull(token);
            Assert.NotEmpty(token);
            Assert.NotNull(refreshToken);
            Assert.NotEmpty(refreshToken);
        }

        [Fact]
        public void GenerateToken_WithDifferentUsers_ReturnsUniqueTokens()
        {
            // Arrange
            var user1 = new User
            {
                Id = 1,
                Email = "test1@example.com",
                FirstName = "Test1",
                LastName = "User1"
            };

            var user2 = new User
            {
                Id = 2,
                Email = "test2@example.com",
                FirstName = "Test2",
                LastName = "User2"
            };

            // Act
            var (token1, refreshToken1) = _jwtUtil.GenerateToken(user1);
            var (token2, refreshToken2) = _jwtUtil.GenerateToken(user2);

            // Assert
            Assert.NotEqual(token1, token2);
            Assert.NotEqual(refreshToken1, refreshToken2);
        }        
    }
} 