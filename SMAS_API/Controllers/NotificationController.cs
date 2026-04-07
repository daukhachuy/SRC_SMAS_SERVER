using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMAS_BusinessObject.DTOs.NotificationDTO;
using SMAS_BusinessObject.Models;
using SMAS_Services.ManagerServices;
using SMAS_Services.NotificationServices;
using System.Security.Claims;

namespace SMAS_API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/notification")]
    public class NotificationController : ControllerBase
    {
        private readonly IManagerService _managerService;
        private readonly INotificationService _notificationService;

        public NotificationController(IManagerService managerService, INotificationService notificationService)
        {
            _managerService = managerService;
            _notificationService = notificationService;
        }

        /// <summary>
        /// Lấy thông báo theo UserId (từ token đăng nhập)
        /// </summary>
        [Authorize(Roles = "Admin,Customer,Waiter,Kitchen,Manager")]
        [HttpGet("unread")]
        public async Task<ActionResult<NotificationDto>> GetNotificationByUserIdAsync()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized();
            }

            var result = await _notificationService.GetNotificationsByUserIdAsync(userId);
            return Ok(result);
        }

        [Authorize(Roles = "Waiter,Kitchen")]
        [HttpPost("change-workshift")]
        public async Task<ActionResult> CreateNotificationChangeWorkStaffAsync([FromBody] ChangeWorkstaffRequestDTO request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized();
            }
            var result = await _notificationService.CreateNotificationAsync( request , userId);
            if (!result)
            {
                return BadRequest(new { MsgCode = "MSG_028", Message = "Không thể yêu cầu đổi lịch vui lòng thử lại !" });
            }
            return Ok(result);
        }


        [Authorize(Roles = "Admin,Customer,Waiter,Kitchen,Manager")]
        [HttpGet("all")]
        public async Task<ActionResult<NotificationDto>> GetAllNotificationAsync()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized();
                }

                var result = await _notificationService.GetAllNotificationAsync(userId);

                if (!result.Any())
                    return Ok(new { MsgCode = "MSG_001", Message = "Không có thông báo nào.", Data = result });

                return Ok(new { MsgCode = "MSG_000", Message = "Lấy danh sách thông báo thành công.", Data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { MsgCode = "MSG_500", Message = "Đã xảy ra lỗi hệ thống.", Detail = ex.Message });
            }
        }
    }
}
