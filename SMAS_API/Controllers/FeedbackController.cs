using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMAS_BusinessObject.DTOs.Feedback;
using SMAS_Services.CustomerFeedbackServices;
using System.Security.Claims;

namespace SMAS_API.Controllers
{
    [ApiController]
    [Route("api/feedback")]
    public class FeedbackController : Controller
    {
        private readonly ICustomerFeedbackService _feedbackService;

        public FeedbackController(ICustomerFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }

        [HttpGet("lists")]
        public async Task<ActionResult<FeedbackListResponse>> GetAllFeedbacks()
        {
            var result = await _feedbackService.GetAllFeedbacksAsync();
            if (result == null || !result.Any())
            {
                return NotFound(new { MsgCode = "MSG_016", Message = "Không có phản hồi nào !" });
            }
            return Ok(result);
        }


        [Authorize(Roles = "Customer")]
        [HttpPost("create-or-update")]
        public async Task<ActionResult> CreateFeedbackAsync([FromBody] CreateFeedbackRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized();
            }
            var result = await _feedbackService.CreateFeedbackAsync(request , userId);
            if (!result.status)
            {
                return BadRequest(result.message);
            }
            return Ok(result.message);

        }
    }
}
