using SMAS_BusinessObject.DTOs.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Repositories.AdminRepository
{
    public interface IAdminRepository
    {
        Task<DashboardSummaryDto> GetSummaryAsync(int month, int year);
        Task<RevenueChartDto> GetRevenueChartAsync(int months);
        Task<OrderStructureDto> GetOrderStructureAsync(int month, int year);
        Task<List<WarehouseTransactionDto>> GetWarehouseTransactionsAsync(int limit);

    }
}
