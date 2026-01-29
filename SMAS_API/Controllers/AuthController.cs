using Google.Apis.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SMAS_BusinessObject.DTOs.Auth;
using SMAS_BusinessObject.Models;
using SMAS_Services.AuthServices;

namespace SMAS_API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : Controller
    {
        private readonly IUserServices _userServices;

        public AuthController(IUserServices userServices)
        {
            _userServices = userServices;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    MsgCode = "MSG_999",
                    Token = (string?)null
                });
            }
            var result = await _userServices.LoginAsync(request);
            return Ok(result);
        }

        [HttpPost("login/google")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    MsgCode = "MSG_999",
                    Token = (string?)null
                });
            }
            var payload = await GoogleJsonWebSignature.ValidateAsync(
                request.Token,
                new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new[] { "YOUR_CLIENT_ID" }
                });

            var result = await _userServices.LoginGoogleAsync(payload.Email);
            return Ok(result);
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    MsgCode = "MSG_999",
                    Token = (string?)null
                });
            }

            var result = await _userServices.RegisterAsync(request);
            return Ok(result);
        }
    
}
}
