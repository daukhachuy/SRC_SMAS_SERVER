using SMAS_BusinessObject.DTOs.CategoryDTO;
using SMAS_BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Repositories.CategoryRepositories
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<CategoryResponse>> GetAllCategoryContainFoodAsync();

        Task<IEnumerable<CategoryResponse>> GetAllCategoriesAsync();
        Task<CategoryResponse?> GetByIdAsync(int id);
        Task<CategoryResponse> CreateAsync(CategoryCreateDto dto);
        Task<CategoryResponse?> UpdateAsync(int id, CategoryUpdateDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> UpdateStatusAsync(int id, bool isAvailable);
    }
}
