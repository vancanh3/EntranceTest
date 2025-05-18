using AuthenticationAPI.Common.Mapper;
using AuthenticationAPI.Common.Utility;
using AuthenticationAPI.Infrastructure;
using AuthenticationAPI.Infrastructure.Entities;
using AuthenticationAPI.Repositories.Interfaces;
using AuthenticationAPI.Services.Data.Request;
using AuthenticationAPI.Services.Data.Response;
using AuthenticationAPI.Services.Interface;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Crypto.Generators;
using System.Text;

namespace AuthenticationAPI.Services.Implement
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly ITokenRepository _tokenRepository;
        private readonly IUserRepository _userRepository;
        private readonly IJwtUtil _jwtUtil;
        private ILogger<AuthenticationService> _logger;

        public AuthenticationService(IJwtUtil jwtUtil, ITokenRepository tokenRepository, IUserRepository userRepository, ILogger<AuthenticationService> logger)
        {
            _jwtUtil = jwtUtil;
            _tokenRepository = tokenRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<RefreshTokenResponse> RefreshToken(string token)
        {
            RefreshTokenResponse refreshTokenResponse = null;

            try
            {
                var storedRefreshToken = _tokenRepository.GetByRefreshToken(token);
                if (storedRefreshToken != null)
                {
                    var expireDate = DateTime.Parse(storedRefreshToken.ExpiresIn);
                    if (expireDate > DateTime.Now)
                    {
                        var user = _userRepository.GetUserById(storedRefreshToken.UserId);
                        if (user != null)
                        {
                            var newTokens = _jwtUtil.GenerateToken(user);
                            var refreshTokenEntity = new Token
                            {
                                UserId = user.Id,
                                RefreshToken = newTokens.Item2,
                                ExpiresIn = DateTime.Now.AddDays(30).ToString(),
                                UpdatedAt = DateTime.Now,
                                CreatedAt = DateTime.Now
                            };
                            
                            _tokenRepository.RemoveOldToken(storedRefreshToken);
                            await _tokenRepository.AddNewTokenAsync(refreshTokenEntity);
                            await _tokenRepository.SaveDataToDbAsync();

                            refreshTokenResponse = new RefreshTokenResponse
                            {
                                RefreshToken = newTokens.Item2,
                                Token = newTokens.Item1,
                            };
                        }
                    }
                }
                else
                {
                    refreshTokenResponse = new RefreshTokenResponse();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            
            return refreshTokenResponse;
        }

        public async Task<bool> RemoveRefreshTokens(int userId)
        {
            var isSucceed = false;
            try
            {
                var tokenOfUsers = await _tokenRepository.GetAllTokensByUserId(userId);
                if (tokenOfUsers.Any())
                {
                    _tokenRepository.RemoveListTokens(tokenOfUsers);
                    await _tokenRepository.SaveDataToDbAsync();
                    isSucceed = true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

            return isSucceed;
        }

        public async Task<AuthenticationResponse> SignInAsync(SignInRequest request)
        {
            AuthenticationResponse authenticationResponse = null;
            try
            {
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
                var loginUser = _userRepository.GetUserByEmail(request.Email);
                bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, loginUser?.Hash);
                if (isPasswordValid)
                {
                    var tokens = _jwtUtil.GenerateToken(loginUser);
                    var refreshTokenEntity = new Token
                    {
                        UserId = loginUser.Id,
                        RefreshToken = tokens.Item2,
                        ExpiresIn = DateTime.Now.AddDays(30).ToString(),
                        UpdatedAt = DateTime.Now,
                        CreatedAt = DateTime.Now
                    };

                    authenticationResponse = new AuthenticationResponse
                    {
                        UserResponse = new UserResponseDto
                        {
                            Email = request.Email,
                            FirstName = loginUser.FirstName,
                            LastName = loginUser.LastName
                        },
                        RefreshToken = tokens.Item2,
                        Token = tokens.Item1
                    };
                    await _tokenRepository.AddNewTokenAsync(refreshTokenEntity);
                    await _userRepository.SaveDataToDbAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            
            return authenticationResponse;
        }

        public async Task<UserResponseDto> SignUpAsync(SignUpRequest request)
        {
            UserResponseDto userResponse = null;
            try
            {
                var existedUser = _userRepository.GetUserByEmail(request.Email);
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
                
                if (existedUser == null)
                {
                    var user = new User
                    {
                        Email = request.Email,
                        Hash = hashedPassword,
                        FirstName = request.FirstName,
                        LastName = request.LastName,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    await _userRepository.AddUserAsync(user);
                    var rowAffects = await _userRepository.SaveDataToDbAsync();
                    if (rowAffects > 0)
                    {
                        userResponse = MapperHelper.Map<User, UserResponseDto>(user);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            return userResponse;

        }
    }
}
