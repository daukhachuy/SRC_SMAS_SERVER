using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMAS_Services.AiBaseServices;
using SMAS_Services.ComboServices;
using SMAS_Services.InventoryServices;

namespace SMAS_API.Controllers
{
    [ApiController]
    [Route("api/admin/dashboard")]
    [Authorize(Roles = "Admin")]
    public class AIAssistantController : Controller
    {
        private readonly IAIAnalysisServices _aiservice;

        public AIAssistantController(IAIAnalysisServices aiService)
        {
            _aiservice = aiService;
        }
        // Feedback AI
        [HttpGet("feedback-analysis")]
        public async Task<IActionResult> AnalyzeFeedback()
        {
            try
            {
                var result = await _aiservice.AnalyzeFeedbackLast3Months();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "Lỗi AI Feedback",
                    error = ex.Message
                });
            }
        }

        // Menu AI
        [HttpGet("menu-analysis")]
        public async Task<IActionResult> AnalyzeMenu()
        {
            try
            {
                var result = await _aiservice.AnalyzeMenuLast3Months();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "Lỗi AI Menu",
                    error = ex.Message
                });
            }
        }

        // Combo AI
        [HttpGet("combo-analysis")]
        public async Task<IActionResult> AnalyzeCombo()
        {
            try
            {
                var result = await _aiservice.AnalyzeComboAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "Lỗi AI Combo",
                    error = ex.Message
                });
            }
        }
    }
}
