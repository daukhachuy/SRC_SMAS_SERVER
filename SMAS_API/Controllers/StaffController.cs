using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMAS_BusinessObject.DTOs.StaffDTO;
using SMAS_BusinessObject.DTOs.WorkShiftDTO;
using SMAS_Repositories.StaffRepository;
using SMAS_Services.ManagerServices;
using SMAS_Services.StaffService;

namespace SMAS_API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class StaffController : ControllerBase
    {
        private readonly IWorkStaffService _workStaffService;
        private readonly IManagerService _managerService;
        private readonly IStaffProfileService _staffProfileService;
        private readonly ILogger<StaffController> _logger;

        public StaffController(IWorkStaffService workStaffService,
            IStaffProfileService staffProfileService,
                                   IManagerService managerService,
                                   ILogger<StaffController> logger)
        {
            _staffProfileService = staffProfileService;
            _workStaffService = workStaffService;
            _managerService = managerService;
            _logger = logger;
        }

        /// <summary>
        /// Lấy danh sách nhân viên làm việc hôm nay (Manager dashboard)
        /// </summary>
        [Authorize(Roles = "Manager")]
        [HttpGet("manager/staffs-today")]
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

        //[Authorize(Roles = "Manager,Admin")]
        [HttpGet("filter-by-position")]
        public async Task<IActionResult> GetFilterStaffByPosition([FromQuery] string? positions)
        {
            try
            {
                var positionList = string.IsNullOrEmpty(positions)
                    ? new List<string>()
                    : positions.Split(',').Select(p => p.Trim()).ToList();

                var result = await _workStaffService
                    .GetFilterStaffByPositionAsync(positionList);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lọc nhân viên theo vị trí.");
                return StatusCode(500, "Đã xảy ra lỗi hệ thống.");
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
        [HttpGet("workshift/next-seven-days")]
        public async Task<IActionResult> GetAllWorkNextSevenDayByPosition([FromQuery] string? positions)
        {
            try
            {
                var positionList = string.IsNullOrEmpty(positions)
                    ? new List<string>()
                    : positions.Split(',').ToList();

                var result = await _workStaffService
                    .GetAllWorkNextSevenDayByPositionAsync(positionList);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy lịch làm 7 ngày tới.");
                return StatusCode(500, "Đã xảy ra lỗi hệ thống.");
            }
        }


        //[Authorize(Roles = "Manager,Admin")]
        [HttpGet("workshift/all")]
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

        //[Authorize(Roles = "Manager,Admin")]
        [HttpPost("workshift")]
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


        [Authorize(Roles = "Waiter,Kitchen,Manager")]
        [HttpGet("sum-workshift-thismonth")]
        public async Task<IActionResult> GetSumWorkShiftThisMonthByStaffId()
        {
            try
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                    return Unauthorized("Không xác định được người dùng.");
                var result = await _workStaffService.GetSumWorkShiftThisMonthByJwtIdAsync(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy tổng số ca làm trong tháng của nhân viên.");
                return StatusCode(500, "Đã xảy ra lỗi hệ thống. Vui lòng thử lại sau.");
            }
        }

        [Authorize(Roles = "Waiter,Kitchen,Manager")]
        [HttpGet("sum-timework-thismonth")]
        public async Task<IActionResult> GetWorkShiftThisMonthByStaffId()
        {
            try
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                    return Unauthorized("Không xác định được người dùng.");
                var result = await _workStaffService.GetSumTimeWorkedThisMonthByJwtIdAsync(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy tổng giờ làm trong tháng của nhân viên.");
                return StatusCode(500, "Đã xảy ra lỗi hệ thống. Vui lòng thử lại sau.");
            }
        }

        [Authorize(Roles = "Waiter,Kitchen")]
        [HttpGet("schedule-week-kitchen-waiter")]
        public async Task<ActionResult<ScheduleWorkResponseDTO>> GetScheduleWorkOnWeekbyStaffIdAsync([FromQuery] DateOnly date)
        {
            if (date < DateOnly.FromDateTime(DateTime.Now.AddMonths(-3)) || date > DateOnly.FromDateTime(DateTime.Now.AddMonths(3)))
            {
                return BadRequest("Date ngoài phạm vi cho phép");
            }
            try
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                    return Unauthorized("Không xác định được người dùng.");
                var result = await _workStaffService.GetScheduleWorkOnWeekbyStaffIdAsync(userId, date);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy lịch làm việc tuần cho nhân viên bếp và phục vụ.");
                return StatusCode(500, "Đã xảy ra lỗi hệ thống. Vui lòng thử lại sau.");
            }
        }

        // GET api/staffprofile
        [Authorize(Roles = "Waiter,Kitchen,Manager")]
        [HttpGet("staff-profile")]
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

        //PUT api/staffprofile
        [Authorize(Roles = "Waiter,Kitchen,Manager")]
        [HttpPut("staff-profile")]
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

        [Authorize(Roles = "Waiter,Kitchen")]
        [HttpGet("get-workshift-notwork")]
        public async Task<ActionResult<WorkStaffResponseDTO>> GetWorkShiftThisWeekByStaffId()
        {
            try
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                    return Unauthorized("Không xác định được người dùng.");
                var result = await _workStaffService.GetWorkScheduleNotCheckinByStaff(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy ca làm trong tuần của nhân viên.");
                return StatusCode(500, "Đã xảy ra lỗi hệ thống. Vui lòng thử lại sau.");
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("staffs-list")]
        public async Task<ActionResult<IEnumerable<StaffResponseDTO>>> GetAllAcountStaffAsync()
        {
            var staffs = await _staffProfileService.GetAllAcountStaffAsync();
            if (!staffs.Any()) return NotFound(new { MsgCode = "MSG_041", Message = "Không có tài khoản nhân viên  nào !" });
            return Ok(staffs);
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpGet("staff-filter")]
        public async Task<ActionResult<StaffResponseDTO>> GetFilterStaff([FromBody] FilterAccountStaffRequestDTO filter)
        {
            var staffs = await _staffProfileService.FilterAccountStaffAsync(filter);
            if (!staffs.Any()) return NotFound(new { MsgCode = "MSG_041", Message = "Không có tài khoản nhân viên  nào !" });
            return Ok(staffs);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("create-staff-userid")]
        public async Task<IActionResult> CreateStaffByUserId([FromBody] CreateNewStaffByUseridResquestDTO dto)
        {

            if (!ModelState.IsValid) return BadRequest();

            try
            {
                var result = await _staffProfileService.CreateStaffAsync(dto);

                if (!result)
                {
                    return BadRequest(new
                    {
                        message = "Tạo staff thất bại. Có thể User không tồn tại hoặc đã là Staff."
                    });
                }
                return Ok(new { message = "Tạo staff thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống", detail = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("create-staff-new")]
        public async Task<IActionResult> CreateNewStaff([FromBody] CreateNewStaffRequestDTO dto)
        {

            if (!ModelState.IsValid) return BadRequest();

            try
            {
                var result = await _staffProfileService.CreateStaffWithUserAsync(dto);

                if (!result)
                {
                    return BadRequest(new
                    {
                        message = "Tạo staff thất bại. Có thể User không tồn tại hoặc đã là Staff."
                    });
                }
                return Ok(new { message = "Tạo staff thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống", detail = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin-staff-deatail/{staffId}")]
        public async Task<ActionResult<StaffDetailresponseDTO>> GetStaffDetailByStaffId([FromRoute] int staffId)
        {
            try
            {
                var result = await _staffProfileService.GetStaffDetailToUpdateAsync(staffId);
                if (result == null) return NotFound(new { MsgCode = "MSG_041", Message = "Không tìm thấy nhân viên nào !" });
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy chi tiết nhân viên {StaffId}.", staffId);
                return StatusCode(500, "Lỗi khi lấy chi tiết nhân viên");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("admin-update-staff-deatail")]
        public async Task<ActionResult<StaffDetailresponseDTO>> UpdateStaffDetailByStaffId([FromBody] StaffDetailRequestDTO staff)
        {
            try
            {
                var result = await _staffProfileService.AdminUpdateStaffDetail(staff);
                if (!result) return NotFound(new { MsgCode = "MSG_041", Message = "Không tìm thấy nhân viên nào !" });
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật thông tin nhân viên {StaffId}.", staff.UserId);
                return StatusCode(500, "Lỗi khi lấy chi tiết nhân viên");
            }
        }
    }

}

