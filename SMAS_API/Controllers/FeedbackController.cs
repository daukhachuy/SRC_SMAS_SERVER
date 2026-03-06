using Microsoft.AspNetCore.Mvc;
using SMAS_BusinessObject.DTOs.Feedback;
using SMAS_Services.CustomerFeedbackServices;

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

    }
}
