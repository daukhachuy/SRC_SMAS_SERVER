using Microsoft.AspNetCore.Mvc;
using SMAS_BusinessObject.DTOs.Combo;
using SMAS_Services.ComboServices;

namespace SMAS_API.Controllers
{
    [ApiController]
    [Route("api/combo")]
    public class ComboController : ControllerBase
    {
        private readonly IComboService _comboService;

        public ComboController(IComboService comboService)
        {
            _comboService = comboService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAvailableCombos()
        {
            var combos = await _comboService.GetAvailableCombosAsync();
            if (combos == null || !combos.Any())
            {
                return NotFound(new { MsgCode = "MSG_025", Message = "Không có combo nào đang hoạt động !" });
            }
            return Ok(combos);
        }

        [HttpPost("filter")]
        public async Task<IActionResult> GetCombosFilter([FromQuery] CombosFilterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (request.MinPrice.HasValue &&
               request.MaxPrice.HasValue &&
               request.MinPrice > request.MaxPrice)
            {
                return BadRequest(new
                {
                    message = "MinPrice không được lớn hơn MaxPrice"
                });
            }
            var filteredCombos = await _comboService.GetCombosFilterAsync(request);
            if (filteredCombos == null || !filteredCombos.Any())
            {
                return NotFound(new { MsgCode = "MSG_026", Message = "Không có combo nào phù hợp với tiêu chí lọc !" });
            }
            return Ok(filteredCombos);
        }
      

    }
}
