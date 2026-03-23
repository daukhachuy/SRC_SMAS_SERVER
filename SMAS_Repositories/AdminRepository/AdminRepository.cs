using SMAS_BusinessObject.DTOs.Admin;
using SMAS_DataAccess.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SMAS_DataAccess.DAO.AdminDao;

namespace SMAS_Repositories.AdminRepository
{
    public class AdminRepository : IAdminRepository
    {
        private readonly AdminDAO _dao;

        public AdminRepository(AdminDAO adminDAO)
        {
            _dao = adminDAO;
        }

        public async Task<DashboardSummaryDto> GetSummaryAsync(int month, int year)
        {
            
            var totalRevenue = await _dao.GetTotalRevenueAsync(month, year);
            var warehouseCost = await _dao.GetWarehouseCostAsync(month, year);
            var newContracts = await _dao.GetNewContractsCountAsync(month, year);
            var newCustomers = await _dao.GetNewCustomersCountAsync(month, year);

            return new DashboardSummaryDto
            {
                TotalRevenue = totalRevenue,
                WarehouseCost = warehouseCost,
                NewContracts = newContracts,
                NewCustomers = newCustomers
            };
        }

        public async Task<RevenueChartDto> GetRevenueChartAsync(int months)
        {
            var now = DateTime.Now;

            var endDate = new DateTime(now.Year, now.Month, 1).AddMonths(1).AddDays(-1); // cuối tháng hiện tại
            var startDate = new DateTime(now.Year, now.Month, 1).AddMonths(-(months - 1)); // đầu tháng N tháng trước

            var revenueMap = await _dao.GetMonthlyRevenueAsync(startDate, endDate);
            var costMap = await _dao.GetMonthlyCostAsync(startDate, endDate);

            // Sinh danh sách đủ N tháng, tháng nào không có data thì = 0
            var items = Enumerable.Range(0, months)
                .Select(i =>
                {
                    var date = startDate.AddMonths(i);
                    var key = $"{date.Year}-{date.Month}";
                    return new RevenueChartItemDto
                    {
                        Month = $"T{date.Month}",
                        Revenue = revenueMap.TryGetValue(key, out var rev) ? rev : 0,
                        Cost = costMap.TryGetValue(key, out var cost) ? cost : 0
                    };
                })
                .ToList();

            return new RevenueChartDto { Data = items };
        }

        public async Task<OrderStructureDto> GetOrderStructureAsync(int month, int year)
        {
            var map = await _dao.GetOrderStructureAsync(month, year);
            var eventCount = await _dao.GetEventOrderCountAsync(month, year);

            return new OrderStructureDto
            {
                DineIn = map.TryGetValue("DineIn", out var d) ? d : 0,
                TakeAway = map.TryGetValue("TakeAway", out var t) ? t : 0,
                Delivery = map.TryGetValue("Delivery", out var v) ? v : 0,
                Event = eventCount
            };
        }
        public async Task<List<WarehouseTransactionDto>> GetWarehouseTransactionsAsync(int limit)
        {
            return await _dao.GetRecentWarehouseTransactionsAsync(limit);
        }
    }

}
