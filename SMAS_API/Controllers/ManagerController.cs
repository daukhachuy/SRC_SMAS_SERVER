using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMAS_Services.ManagerServices;
using System.Security.Claims;

namespace SMAS_API.Controllers
{
    [Authorize(Roles = "Manager")]
    [ApiController]
    [Route("api/manager")]
    public class ManagerController : ControllerBase
    {
        private readonly IManagerService _managerService;

        public ManagerController(IManagerService managerService)
        {
            _managerService = managerService;
        }

        /// <summary>
        /// Lấy danh sách đơn hàng được tạo trong ngày hôm nay
        /// </summary>
        [HttpGet("orders-today")]
        public async Task<IActionResult> GetOrderToday()
        {
            var result = await _managerService.GetOrdersTodayAsync();
            return Ok(result);
        }

        /// <summary>
        /// Lấy danh sách bàn trống
        /// </summary>
        [HttpGet("tables-empty")]
        public async Task<IActionResult> GetTableEmpty()
        {
            var result = await _managerService.GetEmptyTablesAsync();
            return Ok(result);
        }

        /// <summary>
        /// Lấy tổng doanh thu 7 ngày gần nhất (theo tuần)
        /// </summary>
        [HttpGet("revenue-previous-seven-days")]
        public async Task<IActionResult> GetRevenuePreviousSevenDay()
        {
            var result = await _managerService.GetRevenuePreviousSevenDaysAsync();
            return Ok(result);
        }

        /// <summary>
        /// Lấy 4 đơn hàng mới tạo gần nhất
        /// </summary>
        [HttpGet("four-newest-orders")]
        public async Task<IActionResult> GetFourNewCreateOrder()
        {
            var result = await _managerService.GetFourNewestOrdersAsync();
            return Ok(result);
        }

        /// <summary>
        /// Lấy danh sách nhân viên làm việc hôm nay
        /// </summary>
        [HttpGet("staff-work-today")]
        public async Task<IActionResult> GetStaffWorkToday()
        {
            var result = await _managerService.GetStaffWorkTodayAsync();
            return Ok(result);
        }

        /// <summary>
        /// Lấy thông báo theo UserId (từ token đăng nhập)
        /// </summary>
        [HttpGet("notifications")]
        public async Task<IActionResult> GetNotificationByUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized();
            }

            var result = await _managerService.GetNotificationsByUserIdAsync(userId);
            return Ok(result);
        }

        /// <summary>
        /// Tổng số lượng đặt bàn trong ngày hôm nay
        /// </summary>
        [HttpGet("reservations/sum-today")]
        public async Task<IActionResult> GetSumReservationToday()
        {
            var result = await _managerService.GetSumReservationTodayAsync();
            return Ok(result);
        }

        /// <summary>
        /// Danh sách đặt bàn chờ Manager xác nhận (Pending)
        /// </summary>
        [HttpGet("reservations/wait-confirm")]
        public async Task<IActionResult> GetReservationWaitConfirm()
        {
            var result = await _managerService.GetReservationsWaitConfirmAsync();
            return Ok(result);
        }

        /// <summary>
        /// Tất cả đặt bàn sắp xếp theo thời gian tạo giảm dần (mới nhất trước)
        /// </summary>
        [HttpGet("reservations/desc-created-at")]
        public async Task<IActionResult> GetAllReservationDESCCreatedAt()
        {
            var result = await _managerService.GetAllReservationsDescCreatedAtAsync();
            return Ok(result);
        }

        /// <summary>
        /// Tất cả đặt sự kiện (BookEvent) sắp xếp theo thời gian tạo tăng dần
        /// </summary>
        [HttpGet("book-events/asc-created-at")]
        public async Task<IActionResult> GetAllBookEventASCCreatedAt()
        {
            var result = await _managerService.GetAllBookEventsAscCreatedAtAsync();
            return Ok(result);
        }

        /// <summary>
        /// Danh sách sự kiện sắp tới (ReservationDate >= hôm nay)
        /// </summary>
        [HttpGet("upcoming-events")]
        public async Task<IActionResult> GetAllUpcomingEvent()
        {
            var result = await _managerService.GetUpcomingEventsAsync();
            return Ok(result);
        }
    }
}
