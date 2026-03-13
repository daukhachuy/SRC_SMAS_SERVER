using Microsoft.EntityFrameworkCore;
using SMAS_BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_DataAccess.DAO
{
    public class InventoryDAO
    {
        private readonly RestaurantDbContext _context;

        public InventoryDAO(RestaurantDbContext context)
        {
            _context = context;
        }

        public async Task<List<Inventory>> GetAllInventoryAsync() // status : Expired/UsedUp/Active
        {
            return await _context.Inventories
                                 .Where(s => s.Status == "Active")  
                                 .Include(i => i.Ingredient).ToListAsync();
        }

        public async Task<List<InventoryLog>> GetAllInventoryLogsAsync()
        {
            return await _context.InventoryLogs
                                    .Include(i => i.User)
                                    .Include(i => i.Inventory)
                                     .ThenInclude(inv => inv.Ingredient)
                                 .ToListAsync();
        }
    }
}
