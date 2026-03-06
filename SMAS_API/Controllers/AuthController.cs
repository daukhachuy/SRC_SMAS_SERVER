using Google.Apis.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SMAS_BusinessObject.DTOs.Auth;
using SMAS_BusinessObject.Models;
using SMAS_Services.AuthServices;
using SMAS_BusinessObject.Configurations;

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
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request, [FromServices] IOptions<GoogleAuthSettings> googleSettings)
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
                    Audience = googleSettings.Value.ClientIds
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

        [HttpPost("register/google")]
        public async Task<IActionResult> RegisterGoogle( [FromBody] GoogleRegisterRequest request, [FromServices] IOptions<GoogleAuthSettings> googleSettings)
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
                new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = googleSettings.Value.ClientIds
                });

            var result = await _userServices.RegisterGoogleAsync(
                payload.Email,
                payload.Name
            );

            return Ok(result);
        }

      
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new LoginResponse { Token = null, MsgCode = "MSG_999" });
            }
            var result = await _userServices.SendForgotPasswordOtpAsync(request.Email);
            return Ok(result);
        }

      
        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new LoginResponse { Token = null, MsgCode = "MSG_999" });
            }
            var result = await _userServices.VerifyOtpAsync(request.Email, request.Otp);
            return Ok(result);
        }

       
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new LoginResponse { Token = null, MsgCode = "MSG_999" });
            }
            var result = await _userServices.ResetPasswordAsync(request.Email, request.Otp, request.NewPassword);
            return Ok(result);
        }
    }
}
