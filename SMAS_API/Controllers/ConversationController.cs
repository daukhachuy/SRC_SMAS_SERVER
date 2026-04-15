using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SMAS_BusinessObject.DTOs.ConversationDTO;
using SMAS_BusinessObject.Models;
using SMAS_Services.ConversationServices;
using System.Security.Claims;

namespace SMAS_API.Controllers
{
    [Route("api/conversation")]
    [ApiController]
    public class ConversationController : Controller
    {
        private readonly IConversationService _conversationService;

        public ConversationController(IConversationService conversationService)
        {
            _conversationService = conversationService;
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("conversations")]
        public async Task<IActionResult> GetConversationsAsync()
        {
            var result = await _conversationService.GetConversationsAsync();
            if (result == null || result.Count == 0)
                return NotFound("Không tìm thấy cuộc hội thoại nào !");
            return Ok(result);
        }
        [Authorize(Roles = "Manager")]
        [HttpPost("manager-create")]
        public async Task<IActionResult> CreateConversationByManagerAsync(int customerid)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized();
            }
            var reasult = await _conversationService.CreateConversationAsync(userId, customerid);
            if (reasult == null) return BadRequest("Không thể tạo cuộc hội thoại mới , Cuộc hội thoại đã tồn tại !");
            return Ok(reasult);
        }

        [Authorize(Roles = "Customer")]
        [HttpGet("customer-message-my")]
        public async Task<IActionResult> GetConversationsByUseridAsync()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized();
            }
            var result = await _conversationService.GetMessagesByidAsync(userId);
            if (result == null || result.Count == 0)
                return NotFound("Không tìm thấy cuộc hội thoại nào !");
            return Ok(result);
        }

        [Authorize(Roles = "Customer")]
        [HttpPost("customer-create")]
        public async Task<IActionResult> CreateConversationByCustomerAsync(int managerid)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized();
            }
            var result = await _conversationService.CreateConversationByCustomerAsync(userId , managerid);
            if (result == null) return BadRequest("Không thể tạo cuộc hội thoại mới , Cuộc hội thoại đã tồn tại !");
            return Ok(result);
        }

        [Authorize(Roles = "Manager")]
        [HttpGet("manager-conversations-my")]
        public async Task<IActionResult> GetConversationsByManageridAsync()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized();
            }
            var result = await _conversationService.GetConversationsByUseridAsync(userId);
            if (result == null || result.Count == 0)
                return NotFound("Không tìm thấy cuộc hội thoại nào !");
            return Ok(result);
        }

        [Authorize(Roles = "Manager,Customer")]
        [HttpGet("{conversationId}/messages")]
        public async Task<IActionResult> GetMessagesByConversationIdAsync(int conversationId)
        {
            if (conversationId <= 0)
                return BadRequest("Không tìm thấy cuộc hội thoại !");
            return Ok(await _conversationService.GetMessagesAsync(conversationId));
        }

        [Authorize(Roles = "Manager,Customer")]
        [HttpPost("send-messages")]
        public async Task<IActionResult> SendMessageAsync(SendMessageRequestDTO request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized();
            }
            if (request.ConversationId <= 0)
                return BadRequest("Dữ liệu không hợp lệ !");
            return Ok(await _conversationService.SendMessageAsync(request , userId));
        }

        [Authorize(Roles = "Manager,Customer")]
        [HttpPut("{conversationId}/read")]
        public async Task<IActionResult> MarkAsReadAsync(int conversationId, int userId)
        {
            if (conversationId <= 0 || userId <= 0)
                return BadRequest("Không tìm thấy cuộc hội thoại !");
            var result = await _conversationService.MarkAsReadAsync(conversationId, userId);
            if (!result)
                return BadRequest("Đánh dấu đã đọc thất bại !");
            return Ok(result);
        }

        [Authorize(Roles = "Customer")]
        [HttpGet("getall-manager")]
        public async Task<IActionResult> GetAllManagerTocreateConversationAsync()
        {
            var result = await _conversationService.GetAllManagerToConversationAsync();
            if (result == null)
            {
                return BadRequest("Hiện tại không tìm thấy quản lý nào ");
            }
            return Ok(result);
        }

        [Authorize(Roles = "Manager")]
        [HttpGet("getall-customer")]
        public async Task<IActionResult> GetAllCustomerTocreateConversationAsync()
        {
            var result = await _conversationService.GetAllCustomerToConversationAsync();
            if (result == null)
            {
                return BadRequest("Hiện tại không tìm thấy khách hàng nào ");
            }
            return Ok(result);
        }
    }
}
