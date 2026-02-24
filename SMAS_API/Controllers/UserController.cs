using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMAS_BusinessObject.DTOs.Profile;
using SMAS_Services.AuthServices;
using System.Security.Claims;

namespace SMAS_API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserServices _userServices;  

        public UserController(IUserServices userServices)
        {
            _userServices = userServices;
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetMyProfile()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized();

            var user = await _userServices.GetUserProfileAsync(userId); 
            if (user == null)
                return NotFound();

            
            return Ok(new
            {
                user.UserId,
                user.Fullname,
                user.Email,
                user.Gender,
                user.Dob,
                user.Phone,
                user.Address,
                user.Avatar,
                user.Role
            });
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized();

            var response = await _userServices.UpdateProfileAsync(userId, request);

            if (response.Token == null)
            {
                return BadRequest(response);
            }

            return Ok(response); 
        }
    }
}