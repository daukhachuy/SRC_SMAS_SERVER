using SMAS_BusinessObject.DTOs.CategoryDTO;
using SMAS_Repositories.CategoryRepositories;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Services.CategoryServices
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<IEnumerable<CategoryResponse>> GetAllCategoryContainFoodAsync()
        {
            return await _categoryRepository.GetAllCategoryContainFoodAsync();
        }

        public async Task<IEnumerable<CategoryResponse>> GetAllCategoriesAsync()
        {
            return await _categoryRepository.GetAllCategoriesAsync();
        }

        public Task<CategoryResponse?> GetByIdAsync(int id)
            => _categoryRepository.GetByIdAsync(id);

        public Task<CategoryResponse> CreateAsync(CategoryCreateDto dto)
            => _categoryRepository.CreateAsync(dto);

        public Task<CategoryResponse?> UpdateAsync(int id, CategoryUpdateDto dto)
            => _categoryRepository.UpdateAsync(id, dto);

        public Task<bool> DeleteAsync(int id)
             => _categoryRepository.DeleteAsync(id);

        public Task<bool> UpdateStatusAsync(int id, bool isAvailable)
            => _categoryRepository.UpdateStatusAsync(id, isAvailable);
    }
}
