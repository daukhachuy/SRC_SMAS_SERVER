using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMAS_BusinessObject.DTOs.IngredientDTO;
using SMAS_Services.FoodServices;
using SMAS_Services.IngredientServices;

namespace SMAS_API.Controllers
{
    [ApiController]
    [Route("api/ingredient")]
    public class IngredientController : Controller
    {

        private readonly IIngredientService _ingredientService;

        public IngredientController(IIngredientService ingredientService)
        {
            _ingredientService = ingredientService;
        }
        [Authorize(Roles = "Admin,Customer,Waiter,Kitchen,Manager")]
        [HttpGet("GetAll")]
        public async Task<ActionResult<IngredientResponseDTO>>   GetAllIngredientAsync()
        {
            var result = await _ingredientService.GetAllIngredientsAsync();
            if (result == null || !result.Any())
            {
                return NotFound(new { MsgCode = "MSG_034", Message = "Không có nguyên liệu nào !" });
            }
            return Ok(result);
        }


    }
}
