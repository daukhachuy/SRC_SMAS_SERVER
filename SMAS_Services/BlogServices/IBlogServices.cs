using SMAS_BusinessObject.DTOs.BlogDTo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Services.BlogServices
{
    public interface IBlogServices
    {
        Task<IEnumerable<BlogResponse>> GetAllAsync();
        Task<BlogResponse?> GetByIdAsync(int id);
        Task<BlogResponse> CreateAsync(BlogCreateDto dto);
        Task<BlogResponse?> UpdateAsync(int id, BlogUpdateDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> UpdateStatusAsync(int id, string status);
    }
}
