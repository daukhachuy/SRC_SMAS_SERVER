using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SMAS_BusinessObject.Configurations;
using SMAS_BusinessObject.DTOs.SalaryDTO;
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
        private readonly AppSettings _appSettings;

        public SalaryRecordController(
            ISalaryRecordService salaryRecordService,
            ILogger<SalaryRecordController> logger,
            IOptions<AppSettings> appSettings)
        {
            _salaryRecordService = salaryRecordService;
            _logger = logger;
            _appSettings = appSettings.Value;
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

        [Authorize(Roles = "Manager,Admin")]
        [HttpPost("calculate")]
        public async Task<IActionResult> TriggerSalaryCalculation([FromBody] TriggerSalaryCalculationRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var count = await _salaryRecordService.CalculateAndSaveMonthlySalaryAsync(
                    request.Month, request.Year,
                    _appSettings.PenaltyPerLateMinute,
                    _appSettings.FullMonthBonusAmount,
                    _appSettings.DefaultSalaryPerHour);

                if (count == 0)
                    return Ok(new { message = $"Lương tháng {request.Month}/{request.Year} đã được tính trước đó hoặc không có nhân viên." });

                return Ok(new { message = $"Đã tính lương tháng {request.Month}/{request.Year} cho {count} nhân viên." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi trigger tính lương.");
                return StatusCode(500, "Đã xảy ra lỗi hệ thống. Vui lòng thử lại sau.");
            }
        }

        [Authorize(Roles = "Manager,Admin")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllSalaryByMonth([FromQuery] int month, [FromQuery] int year)
        {
            if (month < 1 || month > 12)
                return BadRequest(new { message = "Tháng phải từ 1 đến 12." });
            if (year < 2000)
                return BadRequest(new { message = "Năm không hợp lệ." });

            try
            {
                var result = await _salaryRecordService.GetAllSalaryByMonthAsync(month, year);
                if (result == null || !result.Any())
                    return Ok(new { data = (object?)null, message = $"Chưa có dữ liệu lương tháng {month}/{year}." });

                return Ok(new { data = result, message = "Lấy danh sách lương thành công." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách lương tháng {Month}/{Year}.", month, year);
                return StatusCode(500, "Đã xảy ra lỗi hệ thống. Vui lòng thử lại sau.");
            }
        }

        [Authorize(Roles = "Manager,Admin")]
        [HttpGet("{userId:int}/detail")]
        public async Task<IActionResult> GetSalaryDetailByUser([FromRoute] int userId, [FromQuery] int month, [FromQuery] int year)
        {
            if (month < 1 || month > 12)
                return BadRequest(new { message = "Tháng phải từ 1 đến 12." });
            if (year < 2000)
                return BadRequest(new { message = "Năm không hợp lệ." });

            try
            {
                var result = await _salaryRecordService.GetSalaryDetailByUserAndMonthAsync(userId, month, year);
                if (result == null)
                    return NotFound(new { message = $"Không tìm thấy bản ghi lương của nhân viên {userId} tháng {month}/{year}." });

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy chi tiết lương nhân viên {UserId}.", userId);
                return StatusCode(500, "Đã xảy ra lỗi hệ thống. Vui lòng thử lại sau.");
            }
        }

        [Authorize(Roles = "Manager,Admin")]
        [HttpPut("{salaryRecordId:int}/adjust")]
        public async Task<IActionResult> AdjustBonusPenalty([FromRoute] int salaryRecordId, [FromBody] AdjustBonusPenaltyRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var (success, message) = await _salaryRecordService.AdjustBonusPenaltyAsync(
                    salaryRecordId, request.Bonus, request.Penalty);

                if (!success)
                    return BadRequest(new { message });

                return Ok(new { message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi chỉnh Bonus/Penalty cho SalaryRecord {Id}.", salaryRecordId);
                return StatusCode(500, "Đã xảy ra lỗi hệ thống. Vui lòng thử lại sau.");
            }
        }

    }
}
