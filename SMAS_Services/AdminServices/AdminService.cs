using SMAS_BusinessObject.DTOs.Admin;
using SMAS_Repositories.AdminRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Services.AdminServices
{
    public class AdminService: IAdminService
    {
        private readonly IAdminRepository _adminRepository;

        public AdminService(IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }

        public async Task<DashboardSummaryDto> GetSummaryAsync(int? month, int? year)
        {
            var now = DateTime.Now;
            int resolvedMonth = month ?? now.Month;
            int resolvedYear = year ?? now.Year;

            // Validate
            if (resolvedMonth < 1 || resolvedMonth > 12)
                throw new ArgumentException("Tháng không hợp lệ (1-12).");
            if (resolvedYear < 2000 || resolvedYear > now.Year)
                throw new ArgumentException("Năm không hợp lệ.");

            return await _adminRepository.GetSummaryAsync(resolvedMonth, resolvedYear);
        }
        public async Task<RevenueChartDto> GetRevenueChartAsync(int? months, int? year)
        {
            int resolvedMonths = months ?? 6;

            if (resolvedMonths < 1 || resolvedMonths > 12)
                throw new ArgumentException("Số tháng không hợp lệ (1-12).");

            return await _adminRepository.GetRevenueChartAsync(resolvedMonths);
        }

        public async Task<OrderStructureDto> GetOrderStructureAsync(int? month, int? year)
        {
            var now = DateTime.Now;
            int resolvedMonth = month ?? now.Month;
            int resolvedYear = year ?? now.Year;

            if (resolvedMonth < 1 || resolvedMonth > 12)
                throw new ArgumentException("Tháng không hợp lệ (1-12).");
            if (resolvedYear < 2000 || resolvedYear > now.Year)
                throw new ArgumentException("Năm không hợp lệ.");

            return await _adminRepository.GetOrderStructureAsync(resolvedMonth, resolvedYear);
        }

        public async Task<List<WarehouseTransactionDto>> GetWarehouseTransactionsAsync(int? limit)
        {
            int resolvedLimit = limit ?? 5;

            if (resolvedLimit < 1 || resolvedLimit > 100)
                throw new ArgumentException("Limit không hợp lệ (1-100).");

            return await _adminRepository.GetWarehouseTransactionsAsync(resolvedLimit);
        }

    }
}
