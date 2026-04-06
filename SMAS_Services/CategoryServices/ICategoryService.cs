using SMAS_BusinessObject.DTOs.CategoryDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Services.CategoryServices
{
    public interface ICategoryService
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
