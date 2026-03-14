using SMAS_BusinessObject.DTOs.Food;
using SMAS_BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Repositories.FoodRepositories
{
    public interface IFoodRepository
    {
        Task<IEnumerable<FoodListResponse>> GetAllFoodsCategoryAsync();
        Task<IEnumerable<FoodListResponse>> GetTopBestSellersAsync(int topN = 10);
        Task<BuffetDetailResponseDTO?> GetBuffetWithFoodsAsync(int buffetId);
        Task<List<FoodFilterResponseDTO>> FilterFoodsAsync(FoodFilterRequestDTO request);
        Task<FoodListResponse?> GetFoodByIdAsync(int foodId);
        Task<decimal> GetFoodPriceAsync(int foodId);
        Task<bool> UpdateStatusByFoodId(int foodId);
    }
}
