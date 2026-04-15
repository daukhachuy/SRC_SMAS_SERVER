using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMAS_Services.SalaryService;

namespace SMAS_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SalaryRecordController : ControllerBase
    {
        private readonly ISalaryRecordService _salaryRecordService;
        private readonly ILogger<SalaryRecordController> _logger;

        public SalaryRecordController(ISalaryRecordService salaryRecordService,
                                      ILogger<SalaryRecordController> logger)
        {
            _salaryRecordService = salaryRecordService;
            _logger = logger;
        }

        
        [HttpGet("last-six-months")]
        public async Task<IActionResult> GetSalaryLastSixMonths()
        {
            try
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                    return Unauthorized("Không xác định được người dùng.");

                var result = await _salaryRecordService.GetSalaryLastSixMonthsAsync(userId);
                return Ok(result);

              
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy lịch sử lương.");
                return StatusCode(500, "Đã xảy ra lỗi hệ thống. Vui lòng thử lại sau.");
            }
        }

        [HttpGet("current-month-detail")]
        public async Task<IActionResult> GetCurrentMonthlySalaryDetail()
        {
            try
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                    return Unauthorized("Không xác định được người dùng.");

                var result = await _salaryRecordService.GetCurrentMonthlySalaryDetailAsync(userId);
                if (result == null)
                    return NotFound(new { message = "Chưa có dữ liệu lương tháng hiện tại." });

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy chi tiết lương tháng hiện tại.");
                return StatusCode(500, "Đã xảy ra lỗi hệ thống. Vui lòng thử lại sau.");
            }
        }
    }

}
