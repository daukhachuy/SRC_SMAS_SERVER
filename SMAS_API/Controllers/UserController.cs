using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMAS_BusinessObject.DTOs.Auth;
using SMAS_BusinessObject.DTOs.Profile;
using SMAS_BusinessObject.DTOs.StaffDTO;
using SMAS_Services.AuthServices;
using SMAS_Services.StaffService;
using System.Security.Claims;

namespace SMAS_API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly IUserServices _userServices;  
        private readonly IStaffProfileService _staffProfileService;

        public UserController(IUserServices userServices , IStaffProfileService staffProfileService)
        {
            _userServices = userServices;
            _staffProfileService = staffProfileService;
        }

        [HttpGet("profile")]
        public async Task<ActionResult<CustomerDetailResponse>> GetMyProfile()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized();

            var user = await _userServices.GetUserProfileAsync(userId); 
            if (user == null)
            {
                return NotFound(new { MsgCode = "MSG_023", Message = "Không tìm thấy thông tin người dùng !" });
            }
            return Ok(user);

           
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

        [Authorize(Roles = "Admin")]
        [HttpGet("customers-list")]
        public async Task<ActionResult<IEnumerable<CustomerResponseDTO>>> GetAllAcountCustomersAsync()
        {
            var customers = await _staffProfileService.GetAllAcountCustomerAsync();
            if (!customers.Any()) return NotFound(new { MsgCode = "MSG_040", Message = "Không có tài khoản khách hàng nào !" });
            return Ok(customers);
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("update-status-{userid}")]
        public async Task<IActionResult> UpdateCustomerStatusAsync([FromRoute] int userid)
        {
            var result = await _userServices.UpdateStatusUserAsync(userid);
            if (!result)
                return BadRequest(new { MsgCode = "MSG_041", Message ="Cập nhật trạng thái thất bại !"});
            return Ok(new { MsgCode = "MSG_042", Message = "Cập nhật trạng thái tài khoản thành công." });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("customer-filter")]
        public async Task<ActionResult<StaffResponseDTO>> FilterCustomerAsync(bool request)
        {
            var customers = await _staffProfileService.FilterAccountCustomerAsync(request);
            if (!customers.Any()) return NotFound(new { MsgCode = "MSG_041", Message = "Không có tài khoản nhân viên  nào !" });
            return Ok(customers);
        }

    }
}