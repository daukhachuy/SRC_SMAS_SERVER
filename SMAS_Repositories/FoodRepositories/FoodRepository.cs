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
    }
}
