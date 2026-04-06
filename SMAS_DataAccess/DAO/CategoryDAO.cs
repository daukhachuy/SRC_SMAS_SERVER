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

        public async Task<List<Category>> GetAllCategoryContainFoodAsync()
        {
            return await _context.Categories
                .Where(c => c.Foods.Any())
                .ToListAsync();
        }

        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            return await _context.Categories.ToListAsync();

        }
        public async Task<Category?> GetByIdAsync(int id)
        {
            return await _context.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CategoryId == id);
        }

        public async Task<Category> CreateAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<Category> UpdateAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return false;

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }

        // Patch status: chỉ cập nhật IsAvailable
        public async Task<bool> UpdateStatusAsync(int id, bool isAvailable)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return false;

            category.IsAvailable = isAvailable;
            category.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
