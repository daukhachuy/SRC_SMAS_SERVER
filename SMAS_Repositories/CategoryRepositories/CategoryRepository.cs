using SMAS_BusinessObject.DTOs.CategoryDTO;
using SMAS_DataAccess.DAO;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Repositories.CategoryRepositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly CategoryDAO _categoryDAO;

        public CategoryRepository(CategoryDAO categoryDAO)
        {
            _categoryDAO = categoryDAO;
        }

        public async Task<IEnumerable<CategoryResponse>> GetAllCategoryContainFoodAsync() {           
            var categories = await _categoryDAO.GetAllCategoryContainFoodAsync();
            return categories.Select(c => new CategoryResponse
            {
                CategoryId = c.CategoryId,
                Name = c.Name,
                Description = c.Description,
                IsProcessedGoods = c.IsProcessedGoods,
                Image = c.Image,
                IsAvailable = c.IsAvailable,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt

            });
        }

        public async Task<IEnumerable<CategoryResponse>> GetAllCategoriesAsync()
        {
            var categories = await _categoryDAO.GetAllCategoriesAsync();
            return categories.Select(c => new CategoryResponse
            {
                CategoryId = c.CategoryId,
                Name = c.Name,
                Description = c.Description,
                IsProcessedGoods = c.IsProcessedGoods,
                Image = c.Image,
                IsAvailable = c.IsAvailable,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt

            });
        }
    }
}
