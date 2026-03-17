using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMAS_Services.ManagerServices;
using System.Security.Claims;

namespace SMAS_API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/notification")]
    public class NotificationController : ControllerBase
    {
        private readonly IManagerService _managerService;

        public NotificationController(IManagerService managerService)
        {
            _managerService = managerService;
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
    }
}
