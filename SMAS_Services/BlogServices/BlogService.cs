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

        public async Task<IEnumerable<BlogResponse>> GetAllBlogsAsync()
        {
            return await _blogRepository.GetAllBlogsAsync();
        }
        public async Task<BlogResponse?> GetByIdAsync(int id)
        {
            return await _blogRepository.GetByIdAsync(id);
        }

        public async Task<BlogResponse> CreateAsync(BlogCreateDto dto)
        {
            if (dto.PublishedAt.HasValue && dto.PublishedAt < DateTime.UtcNow.Date)
                throw new ArgumentException("PublishedAt cannot be in the past.");

            return await _blogRepository.CreateAsync(dto);
        }

        public async Task<BlogResponse> UpdateAsync(int id, BlogUpdateDto dto)
        {
            if (dto.PublishedAt.HasValue && dto.PublishedAt < DateTime.UtcNow.Date)
                throw new ArgumentException("PublishedAt cannot be in the past.");

            return await _blogRepository.UpdateAsync(id, dto);
        }

        public async Task DeleteAsync(int id)
            => await _blogRepository.DeleteAsync(id);
    }
}
