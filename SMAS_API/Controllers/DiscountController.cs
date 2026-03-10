using Microsoft.AspNetCore.Mvc;
using SMAS_BusinessObject.DTOs.DiscountDTO;
using SMAS_Services.DiscountServices;

namespace SMAS_API.Controllers
{
    [ApiController]
    [Route("api/discount")]
    public class DiscountController : Controller
    {
        private readonly IDiscountService _discountService;

        public DiscountController(IDiscountService discountService)
        {
            _discountService = discountService;
        }

        [HttpGet("lists")]
        public async Task<ActionResult<DiscountResponse>> GetAllDiscounts()
        {
            var result = await _discountService.GetAllDiscountsAsync();
            if (result == null || !result.Any())
            {
                return NotFound(new { MsgCode = "MSG_014", Message = "Không có mã giảm giá nào !" });
            }
            return Ok(result);
        }

        [HttpGet("{code}")]
        public async Task<ActionResult<DiscountResponse>> GetDiscountByCode(string code)
        {
            var result = await _discountService.GetDiscountByCodeAsync(code);
            if (result == null)
            {
                return NotFound(new { MsgCode = "MSG_013", Message = "Mã giảm giá không tồn tại !" });
            }
            return Ok(result);
        }
    }
}
