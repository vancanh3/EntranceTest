using AuthenticationAPI.Services.Data.Request;
using AuthenticationAPI.Services.Data.Response;

namespace AuthenticationAPI.Services.Interface
{
    public interface IAuthenticationService
    {
        Task<UserResponseDto> SignUpAsync(SignUpRequest request);
        Task<AuthenticationResponse> SignInAsync(SignInRequest request);
        Task<bool> RemoveRefreshTokens(int email);
        Task<RefreshTokenResponse> RefreshToken(string token);
    }
}
