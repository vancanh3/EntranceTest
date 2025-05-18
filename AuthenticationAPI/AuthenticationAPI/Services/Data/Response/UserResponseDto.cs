using AuthenticationAPI.Infrastructure.Entities;

namespace AuthenticationAPI.Services.Data.Response
{
    public class UserResponseDto
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string DisplayName => $"{FirstName} {LastName}";
    }

    public class AuthenticationResponse
    {
        public UserResponseDto UserResponse { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
