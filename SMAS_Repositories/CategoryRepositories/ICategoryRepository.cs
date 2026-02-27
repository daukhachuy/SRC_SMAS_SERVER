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
        Task<IEnumerable<CategoryResponse>> GetAllCategoryAsync();
    }
}
