using Microsoft.EntityFrameworkCore;
using SMAS_BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_DataAccess.DAO
{
    public class IngredientDAO
    {

        private readonly RestaurantDbContext _context;

        public IngredientDAO(RestaurantDbContext context)
        {
            _context = context;
        }


        public async Task<List<Ingredient>> GetAllIngredientsAsync()
        {
            return await _context.Ingredients
                .Include(i => i.Inventories.Where(inv => inv.Status == "Active")) // status : Expired/UsedUp/Active
                .ToListAsync();
        }
    }
}
