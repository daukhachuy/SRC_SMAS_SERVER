using SMAS_BusinessObject.DTOs.BlogDTo;
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

    }
}
