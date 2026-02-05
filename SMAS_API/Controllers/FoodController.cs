using Microsoft.AspNetCore.Mvc;
using SMAS_BusinessObject.DTOs.Food;
using SMAS_Services.AuthServices;
using SMAS_Services.FoodServices;

namespace SMAS_API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class FoodController : Controller
    {
        private readonly IFoodService _ifoodservice;

        public FoodController(IFoodService ifoodservice)
        {
            _ifoodservice = ifoodservice;
        }
        [HttpGet("food/category")]
        public async Task<ActionResult<FoodListResponse>> getall()
        { 
            var result = await _ifoodservice.GetAllFoodsCategoryAsync();
            return Ok(result);
        }
        [HttpGet("food/discount")]
        public async Task<ActionResult<FoodListResponse>> getallfooddiscount()
        {
            var result = await _ifoodservice.GetAllFoodsDiscountAsync();
            return Ok(result);
        }
    }
}
