using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SMAS_BusinessObject.DTOs.TableDTO;
using SMAS_BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
namespace SMAS_DataAccess.DAO
{
    public class TableDAO
    {
        private readonly RestaurantDbContext _context;
        private readonly IConfiguration _config;

        public TableDAO(RestaurantDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<Table?> GetTableByCodeAsync(string tableCode)
        {
            if (string.IsNullOrWhiteSpace(tableCode))
                return null;

            // Ưu tiên tìm theo TableId (đang dùng trong QR Code)
            if (int.TryParse(tableCode, out int tableId))
            {
                return await _context.Tables
                    .FirstOrDefaultAsync(t => t.TableId == tableId && t.IsActive != false);
            }

            // Fallback: tìm theo TableName
            return await _context.Tables
                .FirstOrDefaultAsync(t => 
                    t.TableName.ToUpper() == tableCode.ToUpper() && 
                    t.IsActive != false);
        }

        // Cập nhật Status của Table (OPEN / AVAILABLE...)
        public async Task UpdateTableStatusAsync(int tableId, string status)
        {
            var table = await _context.Tables.FindAsync(tableId);
            if (table == null) return;

            table.Status = status;
            table.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
        //public async Task<List<Table>> GetAllTableAsync()
        //{
        //    return await _context.Tables
        //        .AsNoTracking()
        //        .Where(t => t.IsActive == true)
        //        .OrderBy(t => t.TableName)
        //        .ToListAsync();
        //}

        public async Task<List<TableResponseDTO>> GetTablesAsync(string? tableType, string? status)
        {
            var query = _context.Tables
                .Where(t => t.IsActive != false)
                .AsQueryable();

            if (!string.IsNullOrEmpty(tableType))
                query = query.Where(t => t.TableType == tableType);

            if (!string.IsNullOrEmpty(status))
                query = query.Where(t => t.Status == status);

            var tables = await query.OrderBy(t => t.TableId).ToListAsync();

            var tableIds = tables.Select(t => t.TableId).ToList();
            var activeTableOrders = await _context.TableOrders
                .Where(to => tableIds.Contains(to.TableId)
                          && to.LeftAt == null
                          && to.Order.OrderStatus != "Cancelled"
                          && to.Order.OrderStatus != "Closed")
                .Select(to => new
                {
                    to.TableId,
                    to.Order.NumberOfGuests,
                    to.Order.TotalAmount
                })
                .ToListAsync();

            return tables.Select(t =>
            {
                var activeOrder = activeTableOrders.FirstOrDefault(o => o.TableId == t.TableId);
                return new TableResponseDTO
                {
                    TableId = t.TableId,
                    TableName = t.TableName,
                    TableType = t.TableType,
                    NumberOfPeople = t.NumberOfPeople,
                    Status = t.Status,
                    QrCode = t.QrCode,
                    CurrentGuests = activeOrder?.NumberOfGuests ?? 0,
                    CurrentAmount = activeOrder?.TotalAmount ?? 0
                };
            }).ToList();
        }

        private string GenerateQrCodeValue(int tableId)
        {
            var baseUrl = _config["App:FrontendBaseUrl"] ?? "http://localhost:3000";
            return $"{baseUrl.TrimEnd('/')}/table/{tableId}/scan";
        }
        public async Task<Table> CreateTableAsync(Table table)
        {
            _context.Tables.Add(table);
            await _context.SaveChangesAsync(); // lưu trước để có tableId
            table.QrCode = GenerateQrCodeValue(table.TableId);
            await _context.SaveChangesAsync(); // update QrCode
            return table;
        }
        public async Task<string?> GetActiveOrderCodeByTableIdAsync(int tableId)
        {
            return await _context.TableOrders
                .Where(to => to.TableId == tableId
                          && to.LeftAt == null
                          && to.Order.OrderStatus != "Cancelled"
                          && to.Order.OrderStatus != "Closed"
                          && to.Order.OrderStatus != "Completed")
                .Select(to => to.Order.OrderCode)
                .FirstOrDefaultAsync();
        }

        public async Task<Table?> GetTableByIdAsync(int tableId)
        {
            return await _context.Tables
                .FirstOrDefaultAsync(t => t.TableId == tableId && t.IsActive != false);
        }

        public async Task<bool> UpdateTableAsync(Table table)
        {
            _context.Tables.Update(table);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SoftDeleteTableAsync(int tableId)
        {
            var table = await _context.Tables.FindAsync(tableId);
            if (table == null) return false;

            table.IsActive = false;
            table.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }

        // ─── HELPER ──────────────────────────────────────────────────────────

        /// <summary>Kiểm tra bàn có đang được dùng không (tránh xóa bàn đang có khách)</summary>
        public async Task<bool> IsTableOccupiedAsync(int tableId)
        {
            return await _context.TableOrders
                .AnyAsync(to => to.TableId == tableId
                             && to.LeftAt == null
                             && to.Order.OrderStatus != "Cancelled"
                             && to.Order.OrderStatus != "Closed");
        }
    }
}
