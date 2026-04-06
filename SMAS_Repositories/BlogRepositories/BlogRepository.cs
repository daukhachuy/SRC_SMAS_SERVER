using SMAS_BusinessObject.DTOs.BlogDTo;
using SMAS_BusinessObject.Models;
using SMAS_DataAccess.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Repositories.BlogRepositories
{
    public class BlogRepository : IBlogRepository
    {
        private readonly BlogDAO _blogDAO;

        public BlogRepository(BlogDAO blogDAO)
        {
            _blogDAO = blogDAO;
        }

        public async Task<IEnumerable<BlogResponse>> GetAllAsync()
        {
            var blogs = await _blogDAO.GetAllAsync();
            return blogs.Select(MapToResponseDto);
        }

        public async Task<BlogResponse?> GetByIdAsync(int id)
        {
            var blog = await _blogDAO.GetByIdAsync(id);
            return blog == null ? null : MapToResponseDto(blog);
        }

        public async Task<BlogResponse> CreateAsync(BlogCreateDto dto)
        {
            var blog = MapFromCreateDto(dto);
            var created = await _blogDAO.CreateAsync(blog);
            return MapToResponseDto(created);
        }

        public async Task<BlogResponse?> UpdateAsync(int id, BlogUpdateDto dto)
        {
            var blog = await _blogDAO.GetByIdAsync(id);
            if (blog == null) return null;

            MapFromUpdateDto(dto, blog);
            var updated = await _blogDAO.UpdateAsync(blog);
            return MapToResponseDto(updated);
        }

        public Task<bool> DeleteAsync(int id) => _blogDAO.DeleteAsync(id);

        public Task<bool> UpdateStatusAsync(int id, string status) => _blogDAO.UpdateStatusAsync(id, status);

        // -------------------------------------------------------
        // Mapping helpers
        // -------------------------------------------------------

        private static BlogResponse MapToResponseDto(Blog b) => new BlogResponse
        {
            BlogId = b.BlogId,
            Title = b.Title,
            Content = b.Content,
            Image = b.Image,
            ViewCount = b.ViewCount,
            Status = b.Status,
            AuthorId = b.AuthorId,
            CreatedAt = b.CreatedAt,
            UpdatedAt = b.UpdatedAt,
            PublishedAt = b.PublishedAt
        };

        private static Blog MapFromCreateDto(BlogCreateDto dto) => new Blog
        {
            Title = dto.Title,
            Content = dto.Content,
            Image = dto.Image,
            Status = dto.Status ?? "Draft",
            AuthorId = dto.AuthorId,
            ViewCount = 0,
            PublishedAt = dto.PublishedAt,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        private static void MapFromUpdateDto(BlogUpdateDto dto, Blog blog)
        {
            blog.Title = dto.Title;
            blog.Content = dto.Content;
            blog.Image = dto.Image;
            blog.Status = dto.Status;
            blog.PublishedAt = dto.PublishedAt;
            blog.UpdatedAt = DateTime.UtcNow;
        }

    }
}
