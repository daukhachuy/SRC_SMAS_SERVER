using Microsoft.AspNetCore.Mvc;
using SMAS_BusinessObject.DTOs.Auth;
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
    }
}
