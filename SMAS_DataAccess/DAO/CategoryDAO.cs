using Microsoft.EntityFrameworkCore;
using SMAS_BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_DataAccess.DAO
{
    public class CategoryDAO
    {
        private readonly RestaurantDbContext _context;

        public CategoryDAO(RestaurantDbContext context)
        {
            _context = context;
        }

        public async Task<List<Category>> GetAllCategoryAsync()
        {
            return await _context.Categories.ToListAsync();
        }

    }
}
