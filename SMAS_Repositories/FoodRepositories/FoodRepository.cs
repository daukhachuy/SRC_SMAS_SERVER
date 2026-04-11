using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using SMAS_BusinessObject.DTOs.Food;
using SMAS_BusinessObject.Models;
using SMAS_DataAccess.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Repositories.FoodRepositories
{
    public class FoodRepository : IFoodRepository
    {
        private readonly FoodDAO _foodDAO;

        public FoodRepository(FoodDAO foodDAO)
        {
            _foodDAO = foodDAO;
        }
        public async Task<IEnumerable<FoodListResponse>> GetAllAsync()
        {
            var foods = await _foodDAO.GetAllAsync();
            return foods.Select(MapToResponseDto);
        }

        public async Task<FoodListResponse?> GetByIdAsync(int id)
        {
            var food = await _foodDAO.GetByIdAsync(id);
            return food == null ? null : MapToResponseDto(food);
        }

        public async Task<FoodListResponse> CreateAsync(FoodCreateDto dto)
        {
            var food = MapFromCreateDto(dto);
            var created = await _foodDAO.CreateAsync(food, dto.CategoryIds);
            // Reload để lấy Categories đầy đủ
            var reloaded = await _foodDAO.GetByIdAsync(created.FoodId);
            return MapToResponseDto(reloaded!);
        }
        public async Task<FoodListResponse?> UpdateAsync(int id, FoodUpdateDto dto)
        {
            // Dùng tracked version để EF theo dõi thay đổi
            var food = await _foodDAO.GetByIdTrackedAsync(id);
            if (food == null) return null;

            MapFromUpdateDto(dto, food);
            var updated = await _foodDAO.UpdateAsync(food, dto.CategoryIds);

            var reloaded = await _foodDAO.GetByIdAsync(updated.FoodId);
            return MapToResponseDto(reloaded!);
        }
        public Task<bool> DeleteAsync(int id)
                   => _foodDAO.DeleteAsync(id);

        public Task<bool> UpdateStatusAsync(int id, bool isAvailable)
            => _foodDAO.UpdateStatusAsync(id, isAvailable);

        // -------------------------------------------------------
        // Mapping helpers
        // -------------------------------------------------------

        private static FoodListResponse MapToResponseDto(Food food) => new FoodListResponse
        {
            FoodId = food.FoodId,
            Name = food.Name,
            Description = food.Description,
            Price = food.Price,
            PromotionalPrice = food.PromotionalPrice,
            Image = food.Image,
            Unit = food.Unit,
            IsAvailable = food.IsAvailable,
            IsDirectSale = food.IsDirectSale,
            IsFeatured = food.IsFeatured,
            PreparationTime = food.PreparationTime,
            Calories = food.Calories,
            ViewCount = food.ViewCount,
            OrderCount = food.OrderCount,
            Rating = food.Rating,
            Note = food.Note,
            CreatedAt = food.CreatedAt,
            UpdatedAt = food.UpdatedAt,
            Categories = food.Categories?.Select(c => new CategoryFoodListResponse
            {
                CategoryId = c.CategoryId,
                Name = c.Name,
                Description = c.Description,
                IsProcessedGoods = c.IsProcessedGoods,
                Image = c.Image,
                IsAvailable = c.IsAvailable,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            }).ToList() ?? new List<CategoryFoodListResponse>()
        };

        private static Food MapFromCreateDto(FoodCreateDto dto) => new Food
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            Image = dto.Image,
            Unit = dto.Unit,
            IsAvailable = dto.IsAvailable ?? true,
            IsDirectSale = dto.IsDirectSale,
            IsFeatured = dto.IsFeatured,
            PreparationTime = dto.PreparationTime,
            Calories = dto.Calories,
            Note = dto.Note,
            ViewCount = 0,
            OrderCount = 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow

        };

        // Cập nhật trực tiếp vào entity đã có (tránh tạo object mới mất FoodId)
        private static void MapFromUpdateDto(FoodUpdateDto dto, Food food)
        {
            food.Name = dto.Name;
            food.Description = dto.Description;
            food.Price = dto.Price;
            food.PromotionalPrice = dto.PromotionalPrice;
            food.Image = dto.Image;
            food.Unit = dto.Unit;
            food.IsAvailable = dto.IsAvailable;
            food.IsDirectSale = dto.IsDirectSale;
            food.IsFeatured = dto.IsFeatured;
            food.PreparationTime = dto.PreparationTime;
            food.Calories = dto.Calories;
            food.Note = dto.Note;
            food.UpdatedAt = DateTime.UtcNow;
        }
        public async Task<IEnumerable<FoodListResponse>> GetAllFoodsCategoryAsync()
        {
            var foods = await _foodDAO.GetAllFoodsCategoryAsync();

            return  foods.Select(f => new FoodListResponse
            {
                FoodId = f.FoodId,
                Name = f.Name,
                Description = f.Description,
                Price = f.Price,
                PromotionalPrice = f.PromotionalPrice,
                Image = f.Image,
                Unit = f.Unit,  
                IsAvailable = f.IsAvailable,
                IsDirectSale = f.IsDirectSale,
                IsFeatured = f.IsFeatured,
                PreparationTime = f.PreparationTime,
                Calories = f.Calories,
                ViewCount = f.ViewCount,
                OrderCount = f.OrderCount,
                Rating = f.Rating,
                Note = f.Note,
                CreatedAt = f.CreatedAt,
                UpdatedAt = f.UpdatedAt,
                Categories = f.Categories.Select(c => new CategoryFoodListResponse
                {
                    CategoryId = c.CategoryId,
                    Name = c.Name,
                    Description = c.Description,
                    IsProcessedGoods = c.IsProcessedGoods,
                    Image = c.Image,
                    IsAvailable = c.IsAvailable,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                }).ToList()
            });
        }
        public async Task<IEnumerable<FoodListResponse>> GetTopBestSellersAsync(int topN = 10)
        {
            var foods = await _foodDAO.GetTopBestSellersAsync(topN);

            return foods.Select(f => new FoodListResponse
            {
                FoodId = f.FoodId,
                Name = f.Name,
                Description = f.Description,
                Price = f.Price,
                PromotionalPrice = f.PromotionalPrice,
                Image = f.Image,
                Unit = f.Unit,
                IsAvailable = f.IsAvailable,
                IsDirectSale = f.IsDirectSale,
                IsFeatured = f.IsFeatured,
                PreparationTime = f.PreparationTime,
                Calories = f.Calories,
                ViewCount = f.ViewCount,
                OrderCount = f.OrderCount ?? 0,   // đảm bảo không null
                Rating = f.Rating,
                Note = f.Note,
                CreatedAt = f.CreatedAt,
                UpdatedAt = f.UpdatedAt,
                Categories = f.Categories?.Select(c => new CategoryFoodListResponse
                {
                    CategoryId = c.CategoryId,
                    Name = c.Name,
                    Description = c.Description,
                    IsProcessedGoods = c.IsProcessedGoods,
                    Image = c.Image,
                    IsAvailable = c.IsAvailable,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                }).ToList() ?? new List<CategoryFoodListResponse>()
            });
        }

        public async Task<BuffetDetailResponseDTO?> GetBuffetWithFoodsAsync(int buffetId)
        {
            var buffet = await _foodDAO.GetBuffetWithFoodsAsync(buffetId);

            if (buffet == null) return null;

            var result = new BuffetDetailResponseDTO
            {
                BuffetId = buffet.BuffetId,
                Name = buffet.Name,
                Description = buffet.Description,
                MainPrice = buffet.MainPrice,
                ChildrenPrice = buffet.ChildrenPrice,
                SidePrice = buffet.SidePrice,
                Image = buffet.Image,

                Foods = buffet.BuffetFoods.Select(bf => new BuffetFoodResponseDTO
                {
                    FoodId = bf.Food.FoodId,
                    FoodName = bf.Food.Name,
                    Price = bf.Food.Price,
                    Image = bf.Food.Image,
                    Quantity = bf.Quantity,
                    IsUnlimited = bf.IsUnlimited
                }).ToList()
            };

            return result;
        }


        public async Task<List<FoodFilterResponseDTO>> FilterFoodsAsync(FoodFilterRequestDTO request)
        {
            var foods = await _foodDAO.FilterFoodsAsync(
                request.CategoryIds,
                request.MinPrice,
                request.MaxPrice);

            return foods.Select(f => new FoodFilterResponseDTO
            {
                FoodId = f.FoodId,
                Name = f.Name,
                Description = f.Description,
                Price = f.Price,
                PromotionalPrice = f.PromotionalPrice,
                Image = f.Image,
                Unit = f.Unit,
                Rating = f.Rating,
                Note = f.Note
            }).ToList();
        }

        public async Task<FoodListResponse?> GetFoodByIdAsync(int foodId)
        {
            var f = await _foodDAO.GetFoodByIdAsync(foodId);
            if (f == null) return null;
            return new FoodListResponse
            {
                FoodId = f.FoodId,
                Name = f.Name,
                Description = f.Description,
                Price = f.Price,
                PromotionalPrice = f.PromotionalPrice,
                Image = f.Image,
                Unit = f.Unit,
                IsAvailable = f.IsAvailable,
                IsDirectSale = f.IsDirectSale,
                IsFeatured = f.IsFeatured,
                PreparationTime = f.PreparationTime,
                Calories = f.Calories,
                ViewCount = f.ViewCount,
                OrderCount = f.OrderCount,
                Rating = f.Rating,
                Note = f.Note,
                CreatedAt = f.CreatedAt,
                UpdatedAt = f.UpdatedAt,
                Categories = f.Categories?.Select(c => new CategoryFoodListResponse
                {
                    CategoryId = c.CategoryId,
                    Name = c.Name,
                    Description = c.Description,
                    IsProcessedGoods = c.IsProcessedGoods,
                    Image = c.Image,
                    IsAvailable = c.IsAvailable,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                }).ToList() ?? new List<CategoryFoodListResponse>()
            };
        }

        public async Task<decimal> GetFoodPriceAsync(int foodId)
        {
            return await _foodDAO.GetFoodPriceAsync(foodId);
        }
        public async Task<bool> UpdateStatusByFoodId(int foodId)
        {
            return await _foodDAO.UpdateStatusByFoodId(foodId);
        }

    }
}
