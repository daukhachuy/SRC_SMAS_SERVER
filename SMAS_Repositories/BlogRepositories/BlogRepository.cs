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

        public async Task<IEnumerable<BlogResponse>> GetAllBlogsAsync()
        {
            var blogs = await _blogDAO.GetAllBlogsAsync();
            return blogs.Select(b => new BlogResponse
            {
                BlogId = b.BlogId,
                Title = b.Title,
                Content = b.Content,
                Image = b.Image,
                ViewCount = b.ViewCount,
                Status = b.Status,
                CreatedAt = b.CreatedAt,
                UpdatedAt = b.UpdatedAt,
                PublishedAt = b.PublishedAt,
                Fullname = b.Author.Fullname
            });
        }
        public async Task<BlogResponse?> GetByIdAsync(int id)
        {
            var entity = await _blogDAO.GetByIdAsync(id);
            return entity is null ? null : MapToDto(entity);
        }

        public async Task<BlogResponse> CreateAsync(BlogCreateDto dto)
        {
            var entity = MapToEntity(dto);
            var created = await _blogDAO.CreateAsync(entity);
            return MapToDto(created);
        }

        public async Task<BlogResponse> UpdateAsync(int id, BlogUpdateDto dto)
        {
            var entity = await _blogDAO.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Blog with id {id} not found.");

            ApplyUpdate(entity, dto);
            var updated = await _blogDAO.UpdateAsync(entity);
            return MapToDto(updated);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _blogDAO.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Blog with id {id} not found.");

            // SOFT DELETE
            entity.Status = "Disabled";
            await _blogDAO.UpdateAsync(entity);
        }

        // ==================== MAPPERS ====================
        private static BlogResponse MapToDto(Blog e) => new()
        {
            BlogId = e.BlogId,
            Title = e.Title,
            Content = e.Content,
            Image = e.Image,
            ViewCount = e.ViewCount,
            Status = e.Status,
            CreatedAt = e.CreatedAt,
            UpdatedAt = e.UpdatedAt,
            PublishedAt = e.PublishedAt,
            AuthorId = e.AuthorId
        };

        private static Blog MapToEntity(BlogCreateDto dto) => new()
        {
            Title = dto.Title.Trim(),
            Content = dto.Content,
            Image = dto.Image,
            ViewCount = dto.ViewCount ?? 0,
            Status = dto.Status ?? "Active",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            PublishedAt = dto.PublishedAt,
            AuthorId = dto.AuthorId
        };

        private static void ApplyUpdate(Blog entity, BlogUpdateDto dto)
        {
            entity.Title = dto.Title.Trim();
            entity.Content = dto.Content;
            entity.Image = dto.Image;
            entity.ViewCount = dto.ViewCount ?? entity.ViewCount;
            entity.Status = dto.Status;
            entity.PublishedAt = dto.PublishedAt;
            entity.UpdatedAt = DateTime.UtcNow;
        }

    }
}
