using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMAS_BusinessObject.DTOs.StaffDTO;
using SMAS_Services.ManagerServices;
using SMAS_Services.StaffService;

namespace SMAS_API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class WorkStaffController : ControllerBase
    {
        private readonly IWorkStaffService _workStaffService;
        private readonly IManagerService _managerService;
        private readonly ILogger<WorkStaffController> _logger;

        public WorkStaffController(IWorkStaffService workStaffService,
                                   IManagerService managerService,
                                   ILogger<WorkStaffController> logger)
        {
            _workStaffService = workStaffService;
            _managerService = managerService;
            _logger = logger;
        }

        /// <summary>
        /// Lấy danh sách nhân viên làm việc hôm nay (Manager dashboard)
        /// </summary>
        [Authorize(Roles = "Manager")]
        [HttpGet("staff-work-today")]
        public async Task<IActionResult> GetStaffWorkToday()
        {
            try
            {
                var result = await _managerService.GetStaffWorkTodayAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách nhân viên làm việc hôm nay.");
                return StatusCode(500, "Đã xảy ra lỗi hệ thống. Vui lòng thử lại sau.");
            }
        }

        [Authorize(Roles = "Manager,Admin")]
        [HttpGet("working-today")]
        public async Task<IActionResult> GetStaffWorkingToday() 
        {
            try
            {
                var result = await _workStaffService.GetStaffWorkingTodayAsync();

                if (!result.Any())
                    return Ok(null);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách nhân viên làm việc hôm nay.");
                return StatusCode(500, "Đã xảy ra lỗi hệ thống. Vui lòng thử lại sau.");
            }
        }

        [Authorize(Roles = "Manager,Admin")]
        [HttpPost("filter-by-position")]
        public async Task<IActionResult> GetFilterStaffByPosition([FromBody] List<string> positions)
        {
            try
            {
                // positions rỗng hoặc null → load toàn bộ staff
                var result = await _workStaffService.GetFilterStaffByPositionAsync(positions ?? new List<string>());

                if (!result.Any())
                    return Ok(null);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lọc nhân viên theo vị trí.");
                return StatusCode(500, "Đã xảy ra lỗi hệ thống. Vui lòng thử lại sau.");
            }
        }

        [Authorize(Roles = "Manager,Admin")]
        [HttpGet("{staffId}/work-history")]
        public async Task<IActionResult> GetAllWorkHistoryByStaffId(int staffId, [FromQuery] int month, [FromQuery] int year)
        {
            try
            {
                if (month < 1 || month > 12)
                    return BadRequest("Tháng không hợp lệ. Vui lòng nhập từ 1 đến 12.");

                if (year < 2000)
                    return BadRequest("Năm không hợp lệ.");

                var today = DateTime.Today;
                if (year > today.Year || (year == today.Year && month > today.Month))
                    return BadRequest($"Không thể xem lịch sử trong tương lai. Vui lòng chọn trước tháng {today.Month}/{today.Year}.");

                var result = await _workStaffService.GetAllWorkHistoryByStaffIdAsync(staffId, month, year);

                if (result == null)
                    return Ok(null);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy lịch sử làm việc của nhân viên {StaffId}.", staffId);
                return StatusCode(500, "Đã xảy ra lỗi hệ thống. Vui lòng thử lại sau.");
            }
        }
        [Authorize(Roles = "Manager,Admin")]
        [HttpPost("next-seven-days")]
        public async Task<IActionResult> GetAllWorkNextSevenDayByPosition([FromBody] List<string> positions)
        {
            try
            {
                var result = await _workStaffService.GetAllWorkNextSevenDayByPositionAsync(positions ?? new List<string>());
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy lịch làm 7 ngày tới.");
                return StatusCode(500, "Đã xảy ra lỗi hệ thống. Vui lòng thử lại sau.");
            }
        }

        [Authorize(Roles = "Manager,Admin")]
        [HttpGet("workshift")]
        public async Task<IActionResult> GetAllWorkShift()
        {
            try
            {
                var result = await _workStaffService.GetAllWorkShiftAsync();
                if (!result.Any()) return Ok(null);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách ca làm.");
                return StatusCode(500, "Đã xảy ra lỗi hệ thống. Vui lòng thử lại sau.");
            }
        }

        [Authorize(Roles = "Manager,Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateWorkStaff([FromBody] CreateWorkStaffRequestDto dto)
        {
            try
            {
                var (success, error, data) = await _workStaffService.CreateWorkStaffAsync(dto);

                if (!success)
                    return Conflict(error); // 409 nếu bị trùng ca

                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi phân công ca làm việc.");
                return StatusCode(500, "Đã xảy ra lỗi hệ thống. Vui lòng thử lại sau.");
            }
        }

        [Authorize(Roles = "Manager,Admin")]
        [HttpPut("{workStaffId}")]
        public async Task<IActionResult> UpdateWorkStaff(int workStaffId, [FromBody] UpdateWorkStaffRequestDto dto)
        {
            try
            {
                var (success, error, data) = await _workStaffService.UpdateWorkStaffAsync(workStaffId, dto);

                if (!success)
                    return BadRequest(error);

                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật ca làm việc {WorkStaffId}.", workStaffId);
                return StatusCode(500, "Đã xảy ra lỗi hệ thống. Vui lòng thử lại sau.");
            }
        }

        [Authorize(Roles = "Manager,Admin")]
        [HttpDelete("{workStaffId}")]
        public async Task<IActionResult> DeleteWorkStaff(int workStaffId)
        {
            try
            {
                var (success, error) = await _workStaffService.DeleteWorkStaffAsync(workStaffId);

                if (!success)
                    return NotFound(error);

                return Ok("Xóa ca làm việc thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa ca làm việc {WorkStaffId}.", workStaffId);
                return StatusCode(500, "Đã xảy ra lỗi hệ thống. Vui lòng thử lại sau.");
            }
        }
    }
}
