using AuthenticationAPI.Common.Model;
using AuthenticationAPI.Common.Utility;
using AuthenticationAPI.Infrastructure;
using AuthenticationAPI.Infrastructure.Entities;
using AuthenticationAPI.Repositories.Interfaces;
using AuthenticationAPI.Services.Data.Request;
using AuthenticationAPI.Services.Implement;
using AuthenticationAPI.Services.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AuthenticationAPI.Tests
{
    public class AuthenticationServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<ITokenRepository> _tokenRepositoryMock;
        private readonly Mock<IJwtUtil> _jwtUtilMock;
        private readonly Mock<ILogger<AuthenticationService>> _loggerMock;
        private readonly AuthenticationService _authenticationService;

        public AuthenticationServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _tokenRepositoryMock = new Mock<ITokenRepository>();
            _jwtUtilMock = new Mock<IJwtUtil>();
            _loggerMock = new Mock<ILogger<AuthenticationService>>();

            _authenticationService = new AuthenticationService(
                _jwtUtilMock.Object,
                _tokenRepositoryMock.Object,
                _userRepositoryMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task SignUpAsync_WithValidRequest_ReturnsSuccessResponse()
        {
            // Arrange
            var request = new SignUpRequest
            {
                Email = "test@example.com",
                Password = "Test@123",
                FirstName = "Test",
                LastName = "User"
            };

            _userRepositoryMock.Setup(x => x.SaveDataToDbAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _authenticationService.SignUpAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(request.Email, result.Email);
        }

        [Fact]
        public async Task SignUpAsync_WithExistingEmail_ReturnsNull()
        {
            // Arrange
            var request = new SignUpRequest
            {
                Email = "existing@example.com",
                Password = "Test@123",
                FirstName = "Test",
                LastName = "User"
            };

            _userRepositoryMock.Setup(x => x.GetUserByEmail(It.IsAny<string>()))
                .Returns(new User { Id = 1, Email = request.Email });

            // Act
            var result = await _authenticationService.SignUpAsync(request);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task SignInAsync_WithValidCredentials_ReturnsTokenResponse()
        {
            // Arrange
            var request = new SignInRequest
            {
                Email = "test@example.com",
                Password = "Test@123"
            };

            var user = new User
            {
                Id = 1,
                Email = request.Email,
                Hash = BCrypt.Net.BCrypt.HashPassword(request.Password)
            };

            _userRepositoryMock.Setup(x => x.GetUserByEmail(request.Email))
                .Returns(user);

            _jwtUtilMock.Setup(x => x.GenerateToken(It.IsAny<User>()))
                .Returns(new Tuple<string, string>("test-token", "test-refresh-token"));         

            // Act
            var result = await _authenticationService.SignInAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("test-token", result.Token);
            Assert.Equal("test-refresh-token", result.RefreshToken);
        }

        [Fact]
        public async Task SignInAsync_WithInvalidCredentials_ReturnsNull()
        {
            // Arrange
            var request = new SignInRequest
            {
                Email = "test@example.com",
                Password = "wrongpassword"
            };

            _userRepositoryMock.Setup(x => x.GetUserByEmail(request.Email))
                .Returns((User)null);

            // Act
            var result = await _authenticationService.SignInAsync(request);

            // Assert
            Assert.Null(result);
        }
    }
} 