using SMAS_BusinessObject.DTOs.CategoryDTO;
using SMAS_BusinessObject.Models;
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
        public async Task<CategoryResponse?> GetByIdAsync(int id)
        {
            var category = await _categoryDAO.GetByIdAsync(id);
            return category == null ? null : MapToResponseDto(category);
        }

        public async Task<CategoryResponse> CreateAsync(CategoryCreateDto dto)
        {
            var category = MapFromCreateDto(dto);
            var created = await _categoryDAO.CreateAsync(category);
            return MapToResponseDto(created);
        }

        public async Task<CategoryResponse?> UpdateAsync(int id, CategoryUpdateDto dto)
        {
            var category = await _categoryDAO.GetByIdAsync(id);
            if (category == null) return null;

            MapFromUpdateDto(dto, category);
            var updated = await _categoryDAO.UpdateAsync(category);
            return MapToResponseDto(updated);
        }

        public Task<bool> DeleteAsync(int id)
                => _categoryDAO.DeleteAsync(id);

        public Task<bool> UpdateStatusAsync(int id, bool isAvailable)
            => _categoryDAO.UpdateStatusAsync(id, isAvailable);


        // -------------------------------------------------------
        // Mapping helpers
        // -------------------------------------------------------

        private static CategoryResponse MapToResponseDto(Category c) => new CategoryResponse
        {
            CategoryId = c.CategoryId,
            Name = c.Name,
            Description = c.Description,
            IsProcessedGoods = c.IsProcessedGoods,
            Image = c.Image,
            IsAvailable = c.IsAvailable,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt
        };

        private static Category MapFromCreateDto(CategoryCreateDto dto) => new Category
        {
            Name = dto.Name,
            Description = dto.Description,
            IsProcessedGoods = dto.IsProcessedGoods,
            Image = dto.Image,
            IsAvailable = dto.IsAvailable ?? true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        private static void MapFromUpdateDto(CategoryUpdateDto dto, Category category)
        {
            category.Name = dto.Name;
            category.Description = dto.Description;
            category.IsProcessedGoods = dto.IsProcessedGoods;
            category.Image = dto.Image;
            category.IsAvailable = dto.IsAvailable;
            category.UpdatedAt = DateTime.UtcNow;
        }
    }
}
