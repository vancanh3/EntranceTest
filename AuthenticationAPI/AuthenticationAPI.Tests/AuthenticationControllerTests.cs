using AuthenticationAPI.Controllers;
using AuthenticationAPI.Infrastructure.Entities;
using AuthenticationAPI.Services.Data.Request;
using AuthenticationAPI.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using AuthenticationAPI.Services.Data.Response;
using Newtonsoft.Json;
using AuthenticationAPI.Tests.ModelResponse;

namespace AuthenticationAPI.Tests
{
    public class AuthenticationControllerTests
    {
        private readonly Mock<IAuthenticationService> _authServiceMock;
        private readonly Mock<ILogger<AuthenticationController>> _loggerMock;
        private readonly AuthenticationController _controller;

        public AuthenticationControllerTests()
        {
            _authServiceMock = new Mock<IAuthenticationService>();
            _loggerMock = new Mock<ILogger<AuthenticationController>>();
            _controller = new AuthenticationController(_loggerMock.Object, _authServiceMock.Object);
        }

        [Fact]
        public async Task SignUpUserAsync_WithValidRequest_ReturnsOkResult()
        {
            // Arrange
            var request = new SignUpRequest
            {
                Email = "test@example.com",
                Password = "Test@123",
                FirstName = "Test",
                LastName = "User"
            };

            var user = new User
            {
                Id = 1,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _authServiceMock.Setup(x => x.SignUpAsync(request))
                .ReturnsAsync(new UserResponseDto() { Email = request.Email });

            // Act
            var result = await _controller.SignUpUserAsync(request) as ObjectResult;

            // Assert
            var mappedObject = JsonConvert.DeserializeObject<SignUpResponse>(
                JsonConvert.SerializeObject(result?.Value)
            );
            Assert.NotNull(mappedObject);
            Assert.Equal(200, mappedObject.StatusCode);
        }

        [Fact]
        public async Task SignUpUserAsync_WithInvalidRequest_ReturnsError()
        {
            // Arrange
            var request = new SignUpRequest
            {
                Email = "test@example.com",
                Password = "Test@123",
                FirstName = "Test",
                LastName = "User"
            };

            // Act
            var result = await _controller.SignUpUserAsync(request) as ObjectResult;

            // Assert
            var mappedObject = JsonConvert.DeserializeObject<SignUpResponse>(
               JsonConvert.SerializeObject(result?.Value)
            );
            Assert.NotNull(mappedObject);
            Assert.Equal(500, (int)mappedObject.StatusCode);
        }

        [Fact]
        public async Task SignInAsync_WithValidCredentials_ReturnsOkResult()
        {
            // Arrange
            var request = new SignInRequest
            {
                Email = "test@example.com",
                Password = "Test@123"
            };

            _authServiceMock.Setup(x => x.SignInAsync(request))
                .ReturnsAsync(new AuthenticationResponse() { RefreshToken = "123", Token = "123" });

            // Act
            var result = await _controller.SignInAsync(request) as ObjectResult;

            // Assert
            var mappedObject = JsonConvert.DeserializeObject<SignUpResponse>(
               JsonConvert.SerializeObject(result?.Value)
            );
            Assert.NotNull(mappedObject);
            Assert.Equal(200, mappedObject.StatusCode);
        }

        [Fact]
        public async Task SignOutAsync_WithValidUser_ReturnsSuccess()
        {
            // Arrange
            var claims = new List<Claim>
            {
                new Claim("UserId", "1")
            };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };

            _authServiceMock.Setup(x => x.RemoveRefreshTokens(1))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.SignOutAsync() as ObjectResult;

            // Assert
            Assert.NotNull(result);
            var mappedObject = JsonConvert.DeserializeObject<SignUpResponse>(
              JsonConvert.SerializeObject(result?.Value)
           );
            Assert.Equal(204, (int)mappedObject.StatusCode);
        }
    }
} 