using Microsoft.EntityFrameworkCore;
using SMAS_BusinessObject.DTOs.Admin;
using SMAS_BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_DataAccess.DAO
{
    public class AdminDao
    {
        public class AdminDAO
        {
            private readonly RestaurantDbContext _context;

            public AdminDAO(RestaurantDbContext context)
            {
                _context = context;
            }


            /// <summary>Tổng doanh thu đơn hàng Completed trong tháng/năm</summary>
            public async Task<decimal> GetTotalRevenueAsync(int month, int year)
            {
                return await _context.Orders
                    .Where(o => o.OrderStatus == "Completed"
                             && o.CreatedAt.Month == month
                             && o.CreatedAt.Year == year)
                    .SumAsync(o => (decimal?)o.TotalAmount) ?? 0;
            }

            /// <summary>Chi phí nhập kho = SUM TotalAmount của Transaction loại Import</summary>
            public async Task<decimal> GetWarehouseCostAsync(int month, int year)
            {
                return await _context.Transactions
                    .Where(t => t.TransactionType == "Import"
                             && t.TransactionDate.Month == month
                             && t.TransactionDate.Year == year)
                    .SumAsync(t => (decimal?)t.TotalAmount) ?? 0;
            }

            /// <summary>Số hợp đồng mới trong tháng/năm</summary>
            public async Task<int> GetNewContractsCountAsync(int month, int year)
            {
                return await _context.Contracts
                    .Where(c => c.CreatedAt.HasValue
                             && c.CreatedAt.Value.Month == month
                             && c.CreatedAt.Value.Year == year)
                    .CountAsync(); 
            }

            /// <summary>Số khách hàng mới trong tháng/năm</summary>
            public async Task<int> GetNewCustomersCountAsync(int month, int year)
            {
                return await _context.Users
                    .Where(u => u.Role == "Customer"
                             && u.IsDeleted != true
                             && u.CreatedAt.HasValue
                             && u.CreatedAt.Value.Month == month
                             && u.CreatedAt.Value.Year == year)
                    .CountAsync(); 
            }

            /// <summary>
            /// Doanh thu theo từng tháng trong khoảng startDate -> endDate.
            /// Key = "yyyy-M" để phân biệt khi vắt qua năm, Value = tổng TotalAmount.
            /// </summary>
            public async Task<Dictionary<string, decimal>> GetMonthlyRevenueAsync(DateTime startDate, DateTime endDate)
            {
                return await _context.Orders
                    .Where(o => o.OrderStatus == "Completed"
                             && o.CreatedAt >= startDate
                             && o.CreatedAt <= endDate)
                    .GroupBy(o => new { o.CreatedAt.Year, o.CreatedAt.Month })
                    .Select(g => new
                    {
                        Key = g.Key.Year + "-" + g.Key.Month,
                        Total = g.Sum(o => o.TotalAmount)
                    })
                    .ToDictionaryAsync(x => x.Key, x => x.Total);
            }

            /// <summary>
            /// Chi phí nhập kho theo từng tháng trong khoảng startDate -> endDate.
            /// </summary>
            public async Task<Dictionary<string, decimal>> GetMonthlyCostAsync(DateTime startDate, DateTime endDate)
            {
                return await _context.Transactions
                    .Where(t => t.TransactionType == "Import"
                             && t.TransactionDate >= startDate
                             && t.TransactionDate <= endDate)
                    .GroupBy(t => new { t.TransactionDate.Year, t.TransactionDate.Month })
                    .Select(g => new
                    {
                        Key = g.Key.Year + "-" + g.Key.Month,
                        Total = g.Sum(t => t.TotalAmount)
                    })
                    .ToDictionaryAsync(x => x.Key, x => x.Total);
            }

            public async Task<Dictionary<string, int>> GetOrderStructureAsync(int month, int year)
            {
                return await _context.Orders
                    .Where(o => o.CreatedAt.Month == month
                             && o.CreatedAt.Year == year
                             && o.OrderType != null)
                    .GroupBy(o => o.OrderType!)
                    .Select(g => new { Type = g.Key, Count = g.Count() })
                    .ToDictionaryAsync(x => x.Type, x => x.Count);
            }

            /// <summary>Số đơn có liên kết sự kiện (BookEventId != null) trong tháng/năm</summary>
            public async Task<int> GetEventOrderCountAsync(int month, int year)
            {
                return await _context.Orders
                    .Where(o => o.CreatedAt.Month == month
                             && o.CreatedAt.Year == year
                             && o.BookEventId != null)
                    .CountAsync();
            }

            /// Lấy N giao dịch nhập kho gần nhất, kèm tên nhà cung cấp.
            public async Task<List<WarehouseTransactionDto>> GetRecentWarehouseTransactionsAsync(int limit)
            {
                return await _context.Transactions
                    .Where(t => t.TransactionType == "Import")
                    .OrderByDescending(t => t.TransactionDate)
                    .Take(limit)
                    .Select(t => new WarehouseTransactionDto
                    {
                        TransactionCode = t.TransactionCode,
                        SupplierName = t.Supplier != null ? t.Supplier.SupplierName : null,
                        TotalAmount = t.TotalAmount,
                        PaymentStatus = t.PaymentStatus,
                        TransactionDate = t.TransactionDate
                    })
                    .ToListAsync();
            }
        }
    }

}
