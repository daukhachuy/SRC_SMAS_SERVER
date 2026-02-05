using SMAS_BusinessObject.DTOs.Food;
using SMAS_Repositories.FoodRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Services.FoodServices
{
    public class FoodService : IFoodService
    {

        private readonly IFoodRepository _foodRepository;

        public FoodService(IFoodRepository foodRepository)
        {
            _foodRepository = foodRepository;
        }

        public async Task<IEnumerable<FoodListResponse>> GetAllFoodsCategoryAsync()
        {
            return await _foodRepository.GetAllFoodsCategoryAsync();
        }

        public async Task<IEnumerable<FoodListResponse>> GetAllFoodsDiscountAsync()
        {
            var foods = await _foodRepository.GetAllFoodsCategoryAsync();

            return foods.Where(f => f.PromotionalPrice.HasValue && 
                                                   f.PromotionalPrice.Value < f.Price);
        }
    }
}
