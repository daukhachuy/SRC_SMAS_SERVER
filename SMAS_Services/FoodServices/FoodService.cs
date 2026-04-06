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
        public Task<IEnumerable<FoodListResponse>> GetAllAsync()
           => _foodRepository.GetAllAsync();

        public Task<FoodListResponse?> GetByIdAsync(int id)
            => _foodRepository.GetByIdAsync(id);

        public Task<FoodListResponse> CreateAsync(FoodCreateDto dto)
            => _foodRepository.CreateAsync(dto);

        public Task<FoodListResponse?> UpdateAsync(int id, FoodUpdateDto dto)
            => _foodRepository.UpdateAsync(id, dto);

        public Task<bool> DeleteAsync(int id)
             => _foodRepository.DeleteAsync(id);

        public Task<bool> UpdateStatusAsync(int id, bool isAvailable)
            => _foodRepository.UpdateStatusAsync(id, isAvailable);
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

        public async Task<IEnumerable<FoodListResponse>> GetTopBestSellersAsync(int top = 10)
        {
            return await _foodRepository.GetTopBestSellersAsync(top);
        }

        public async Task<BuffetDetailResponseDTO?> GetBuffetWithFoodsAsync(int buffetId)
        {
            return await _foodRepository.GetBuffetWithFoodsAsync(buffetId);
        }

        public async Task<List<FoodFilterResponseDTO>> FilterFoodsAsync(FoodFilterRequestDTO request)
        {
            return await _foodRepository.FilterFoodsAsync(request);
        }

        public async Task<FoodListResponse?> GetFoodByIdAsync(int foodId)
        {
            return await _foodRepository.GetFoodByIdAsync(foodId);
        }

        public async Task<decimal> GetFoodPriceAsync(int foodId)
        {
            return await _foodRepository.GetFoodPriceAsync(foodId);
        }
        public async Task<bool> UpdateStatusByFoodId(int foodId)
        {
            return await _foodRepository.UpdateStatusByFoodId(foodId);
        }
    }
}
