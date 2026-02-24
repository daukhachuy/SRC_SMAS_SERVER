using Microsoft.EntityFrameworkCore;
using SMAS_BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_DataAccess.DAO
{
    public class DiscountDao
    {

        private readonly RestaurantDbContext _context;

        public DiscountDao(RestaurantDbContext context)
        {
            _context = context;
        }

        public async Task<List<Discount>> GetAllDiscountsAsync()
        {
            return await _context.Discounts.ToListAsync();
        }
    }
}
