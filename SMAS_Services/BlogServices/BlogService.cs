using SMAS_BusinessObject.DTOs.BlogDTo;
using SMAS_Repositories.BlogRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Services.BlogServices
{
    public class BlogService : IBlogServices
    {

        private readonly IBlogRepository _blogRepository;

        public BlogService(IBlogRepository blogRepository)
        {
            _blogRepository = blogRepository;
        }

        public Task<IEnumerable<BlogResponse>> GetAllAsync() => _blogRepository.GetAllAsync();
        public Task<BlogResponse?> GetByIdAsync(int id) => _blogRepository.GetByIdAsync(id);
        public Task<BlogResponse> CreateAsync(BlogCreateDto dto) => _blogRepository.CreateAsync(dto);
        public Task<BlogResponse?> UpdateAsync(int id, BlogUpdateDto dto) => _blogRepository.UpdateAsync(id, dto);
        public Task<bool> DeleteAsync(int id) => _blogRepository.DeleteAsync(id);
        public Task<bool> UpdateStatusAsync(int id, string status) => _blogRepository.UpdateStatusAsync(id, status);
    }
}
