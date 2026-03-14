using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMAS_Repositories.StaffRepository;
using SMAS_Services.StaffService;

namespace SMAS_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class StaffProfileController : ControllerBase
    {
        private readonly IStaffProfileService _staffProfileService;
        private readonly ILogger<StaffProfileController> _logger;

        public StaffProfileController(IStaffProfileService staffProfileService,
                                      ILogger<StaffProfileController> logger)
        {
            _staffProfileService = staffProfileService;
            _logger = logger;
        }

        // GET api/staffprofile
        [HttpGet]
        public async Task<IActionResult> GetProfileStaff()
        {
            try
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                    return Unauthorized("Không xác định được người dùng.");

                var result = await _staffProfileService.GetProfileStaffAsync(userId);

                if (result == null) return Ok(null);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy hồ sơ nhân viên.");
                return StatusCode(500, "Đã xảy ra lỗi hệ thống. Vui lòng thử lại sau.");
            }
        }

        // PUT api/staffprofile
        [HttpPut]
        public async Task<IActionResult> UpdateProfileStaff([FromBody] UpdateProfileStaffRequestDto dto)
        {
            try
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                    return Unauthorized("Không xác định được người dùng.");

                var (success, error) = await _staffProfileService.UpdateProfileStaffAsync(userId, dto);

                if (!success) return BadRequest(error);

                return Ok("Cập nhật hồ sơ thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật hồ sơ nhân viên.");
                return StatusCode(500, "Đã xảy ra lỗi hệ thống. Vui lòng thử lại sau.");
            }
        }
    }

}
