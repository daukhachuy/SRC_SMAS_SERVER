using SMAS_BusinessObject.DTOs.SalaryDTO;
using SMAS_BusinessObject.Models;

namespace SMAS_Repositories.SalaryRepository
{
    public interface ISalaryRecordRepository
    {
        Task<SalaryLastSixMonthsResponseDto> GetSalaryLastSixMonthsAsync(int userId);
        Task<MonthlySalaryDetailResponseDto?> GetCurrentMonthlySalaryDetailAsync(int userId);
        Task<bool> ExistsAsync(int month, int year);
        Task CreateBatchAsync(List<SalaryRecord> records);
        Task<List<Staff>> GetAllActiveStaffAsync();
        Task<List<WorkStaff>> GetWorkStaffByMonthAsync(int userId, int month, int year);
        Task<List<SalaryRecordListItemDto>> GetAllByMonthAsync(int month, int year);
        Task<MonthlySalaryDetailResponseDto?> GetByUserAndMonthAsync(int userId, int month, int year);
        Task<SalaryRecord?> GetByIdAsync(int salaryRecordId);
        Task UpdateAsync(SalaryRecord record);
    }
}
