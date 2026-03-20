using Microsoft.EntityFrameworkCore;
using SMAS_BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_DataAccess.DAO
{
    public class TableDAO
    {
        private readonly RestaurantDbContext _context;

        public TableDAO(RestaurantDbContext context)
        {
            _context = context;
        }

        public async Task<Table?> GetTableByCodeAsync(string tableCode)
        {
            return await _context.Tables
                .FirstOrDefaultAsync(t => t.TableName == tableCode && t.IsActive == true);
        }

        // Cập nhật Status của Table (OPEN / CLOSED / AVAILABLE...)
        public async Task UpdateTableStatusAsync(int tableId, string status)
        {
            var table = await _context.Tables.FindAsync(tableId);
            if (table == null) return;

            table.Status = status;
            table.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
        public async Task<List<Table>> GetAllTableAsync()
        {
            return await _context.Tables
                .AsNoTracking()
                .Where(t => t.IsActive == true)
                .OrderBy(t => t.TableName)
                .ToListAsync();
        }
    }
}
