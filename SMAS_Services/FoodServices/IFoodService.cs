using SMAS_BusinessObject.DTOs.Food;
using SMAS_BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Services.FoodServices
{
    public interface IFoodService
    {
        Task<IEnumerable<FoodListResponse>> GetAllFoodsCategoryAsync();

        Task<IEnumerable<FoodListResponse>> GetAllFoodsDiscountAsync();

        Task<IEnumerable<FoodListResponse>> GetTopBestSellersAsync(int top = 10);

        Task<BuffetDetailResponseDTO?> GetBuffetWithFoodsAsync(int buffetId);
        Task<List<FoodFilterResponseDTO>> FilterFoodsAsync(FoodFilterRequestDTO request);
    }
}
