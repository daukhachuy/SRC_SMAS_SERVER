using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMAS_BusinessObject.DTOs.NotificationDTO;
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

        public NotificationController(IManagerService managerService , INotificationService notificationService )
        {
            _managerService = managerService;
            _notificationService = notificationService;
        }

        /// <summary>
        /// Lấy thông báo theo UserId (từ token đăng nhập)
        /// </summary>
        [Authorize(Roles = "Manager")]
        [HttpGet]
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

        [Authorize(Roles = "Waiter,Kitchen")]
        [HttpPost("change-workshift")]
        public async Task<IActionResult> CreateNotificationChangeWorkStaffAsync([FromBody] ChangeWorkstaffRequestDTO request)
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
    }
}
