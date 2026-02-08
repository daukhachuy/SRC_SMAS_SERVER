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
    }
}
