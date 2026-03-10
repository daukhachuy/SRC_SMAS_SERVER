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

        public async Task<IEnumerable<CategoryResponse>> GetAllCategoryAsync()
        {
            return await _categoryRepository.GetAllCategoryAsync();
        }
    }
}
