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
            return Ok(result);
        }
    }
}
