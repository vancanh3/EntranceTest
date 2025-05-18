using AuthenticationAPI.Common.Utility;
using AuthenticationAPI.Customize;
using AuthenticationAPI.Services.Data.Request;
using AuthenticationAPI.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using Org.BouncyCastle.Asn1.Ocsp;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AuthenticationAPI.Controllers
{
    [Route("[controller]")]
    public class AuthenticationController : Controller
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly ILogger<AuthenticationController> _logger;

        public AuthenticationController(ILogger<AuthenticationController> logger, IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
            _logger = logger;
        }

        [HttpPost("signup")]
        [ValidateDataRequest]

        public async Task<IActionResult> SignUpUserAsync([FromBody] SignUpRequest request)
        {
            int statusCode = (int)ConstantsUtil.HttpStatusCode.Ok;
            var data = await _authenticationService.SignUpAsync(request);
            if (data == null)
            {
                statusCode = (int)ConstantsUtil.HttpStatusCode.InternalServerError;
                return new ObjectResult(new { statusCode, message = "Error", currentDate = DateTime.Now, });
            }
            return new ObjectResult(new { statusCode, data, currentDate = DateTime.Now, });
        }

        [HttpPost("signin")]
        [ValidateDataRequest]

        public async Task<IActionResult> SignInAsync([FromBody] SignInRequest request)
        {
            int statusCode = (int)ConstantsUtil.HttpStatusCode.Ok;
            var data = await _authenticationService.SignInAsync(request);
            if (data == null)
            {
                statusCode = (int)ConstantsUtil.HttpStatusCode.InternalServerError;
                return new ObjectResult(new { statusCode, data = "Error", currentDate = DateTime.Now, });
            }
            return new ObjectResult(new { statusCode, data, currentDate = DateTime.Now, });
        }

        [HttpPost("signout")]
        [Authorize]
        public async Task<IActionResult> SignOutAsync()
        {
            int statusCode = (int)ConstantsUtil.HttpStatusCode.Success;
            var userIdString = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (!int.TryParse(userIdString, out int userId))
            {
                statusCode = (int)ConstantsUtil.HttpStatusCode.InternalServerError;
                return new ObjectResult(new { statusCode, data = "Error", currentDate = DateTime.Now, });
            }

            var data = await _authenticationService.RemoveRefreshTokens(userId);
            return new ObjectResult(new { statusCode, currentDate = DateTime.Now, });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RenewToken([FromBody] string refreshToken)
        {
            int statusCode = (int)ConstantsUtil.HttpStatusCode.Ok;
            var data = await _authenticationService.RefreshToken(refreshToken);
            ObjectResult objectResult = null;
            if (data == null || data.Token == null)
            {
                statusCode = (int)ConstantsUtil.HttpStatusCode.InternalServerError;
                if (data.Token == null)
                {
                    statusCode = (int)ConstantsUtil.HttpStatusCode.InvalidToken;                    
                }
                
                return new ObjectResult(new { statusCode, message = "Error", currentDate = DateTime.Now, });
            }
            return new ObjectResult(new { statusCode, data, currentDate = DateTime.Now, });
        }
    }
}
