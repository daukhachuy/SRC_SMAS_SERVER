using SMAS_BusinessObject.DTOs.SalaryDTO;
using SMAS_BusinessObject.Models;
using SMAS_DataAccess.DAO;

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

        public async Task<bool> ExistsAsync(int month, int year)
            => await _salaryRecordDAO.ExistsAsync(month, year);

        public async Task CreateBatchAsync(List<SalaryRecord> records)
            => await _salaryRecordDAO.CreateBatchAsync(records);

        public async Task<List<Staff>> GetAllActiveStaffAsync()
            => await _salaryRecordDAO.GetAllActiveStaffAsync();

        public async Task<List<WorkStaff>> GetWorkStaffByMonthAsync(int userId, int month, int year)
            => await _salaryRecordDAO.GetWorkStaffByMonthAsync(userId, month, year);

        public async Task<List<SalaryRecordListItemDto>> GetAllByMonthAsync(int month, int year)
        {
            var records = await _salaryRecordDAO.GetAllByMonthAsync(month, year);
            return records.Select(s => new SalaryRecordListItemDto
            {
                SalaryRecordId = s.SalaryRecordId,
                UserId = s.UserId,
                Fullname = s.User?.Fullname,
                Position = s.User?.Staff?.Position,
                TotalWorkingDay = s.TotalWorkingDay,
                TotalWorkingHours = s.TotalWorkingHours,
                BaseSalary = s.BaseSalary,
                Bonus = s.Bonus,
                Penalty = s.Penalty,
                TotalSalary = s.TotalSalary,
                PaymentStatus = s.PaymentStatus
            }).ToList();
        }

        public async Task<MonthlySalaryDetailResponseDto?> GetByUserAndMonthAsync(int userId, int month, int year)
        {
            var record = await _salaryRecordDAO.GetByUserAndMonthAsync(userId, month, year);
            if (record == null) return null;
            return MapToDetailDto(record);
        }

        public async Task<SalaryRecord?> GetByIdAsync(int salaryRecordId)
            => await _salaryRecordDAO.GetByIdAsync(salaryRecordId);

        public async Task UpdateAsync(SalaryRecord record)
            => await _salaryRecordDAO.UpdateAsync(record);

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
