using Microsoft.EntityFrameworkCore;
using SMAS_BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_DataAccess.DAO
{
    public class BlogDAO
    {
        private readonly RestaurantDbContext _context;

        public BlogDAO(RestaurantDbContext context)
        {
            _context = context;
        }

        public async Task<List<Blog>> GetAllBlogsAsync()
        {
            return await _context.Blogs.Include(u => u.Author).ToListAsync();
        }
        public async Task<Blog?> GetByIdAsync(int id)
        {
            return await _context.Blogs
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.BlogId == id);
        }

        public async Task<Blog> CreateAsync(Blog blog)
        {
            _context.Blogs.Add(blog);
            await _context.SaveChangesAsync();
            return blog;
        }

        public async Task<Blog> UpdateAsync(Blog blog)
        {
            _context.Blogs.Update(blog);
            await _context.SaveChangesAsync();
            return blog;
        }

        public async Task<bool> UpdateStatusAsync(int id)
        {
            var blog = await _context.Blogs.FindAsync(id);
            if (blog == null) return false;
            if (blog.Status == "Published")
            {
                blog.Status = "Draft";
            }
            else
            {
                blog.Status = "Published";
            }
            _context.Blogs.Update(blog);
            return await _context.SaveChangesAsync() > 0;

        }
    }
}
