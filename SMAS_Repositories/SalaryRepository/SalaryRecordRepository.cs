using SMAS_BusinessObject.DTOs.SalaryDTO;
using SMAS_BusinessObject.Models;
using SMAS_DataAccess.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Repositories.SalaryRepository
{
    public class SalaryRecordRepository : ISalaryRecordRepository
    {
        private readonly SalaryRecordDAO _salaryRecordDAO;

        public SalaryRecordRepository(SalaryRecordDAO salaryRecordDAO)
        {
            _salaryRecordDAO = salaryRecordDAO;
        }

        public async Task<SalaryLastSixMonthsResponseDto> GetSalaryLastSixMonthsAsync(int userId)
        {
            var records = await _salaryRecordDAO.GetSalaryLastSixMonthsAsync(userId);
            var months = records.Select(s => MapToMonthDto(s)).ToList();
            var average = months.Any() ? Math.Round(months.Average(m => m.TotalSalary), 3) : 0;

            return new SalaryLastSixMonthsResponseDto
            {
                AverageSalary = average,
                Months = months
            };
        }

        public async Task<MonthlySalaryDetailResponseDto?> GetCurrentMonthlySalaryDetailAsync(int userId)
        {
            var record = await _salaryRecordDAO.GetCurrentMonthlySalaryDetailAsync(userId);
            if (record == null) return null;
            return MapToDetailDto(record);
        }

        // ---------------------------------------------------------------
        private static SalaryMonthDto MapToMonthDto(SalaryRecord s) =>
            new()
            {
                Month = s.Month,
                Year = s.Year,
                TotalSalary = s.TotalSalary,
                PaymentStatus = s.PaymentStatus,
                PaidAt = s.CreatedAt
            };

        private static MonthlySalaryDetailResponseDto MapToDetailDto(SalaryRecord s) =>
            new()
            {
                Month = s.Month,
                Year = s.Year,
                PaymentStatus = s.PaymentStatus,
                BaseSalary = s.BaseSalary,
                ExperienceLevel = s.User?.Staff?.ExperienceLevel,
                Bonus = s.Bonus,
                Penalty = s.Penalty,
                TotalWorkingDay = s.TotalWorkingDay,
                TotalWorkingHours = s.TotalWorkingHours,
                TotalSalary = s.TotalSalary
            };
    }
}
