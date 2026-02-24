using Microsoft.AspNetCore.Mvc;
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
            return Ok(combos);
        }

        
    }
}
