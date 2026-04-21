using SMAS_BusinessObject.DTOs.SalaryDTO;

namespace SMAS_Services.SalaryService
{
    public interface ISalaryRecordService
    {
        Task<SalaryLastSixMonthsResponseDto> GetSalaryLastSixMonthsAsync(int userId);
        Task<MonthlySalaryDetailResponseDto?> GetCurrentMonthlySalaryDetailAsync(int userId);
        Task<int> CalculateAndSaveMonthlySalaryAsync(int month, int year, decimal penaltyPerLateMinute, decimal fullMonthBonus, decimal defaultSalaryPerHour);
        Task<List<SalaryRecordListItemDto>> GetAllSalaryByMonthAsync(int month, int year);
        Task<MonthlySalaryDetailResponseDto?> GetSalaryDetailByUserAndMonthAsync(int userId, int month, int year);
        Task<(bool Success, string Message)> AdjustBonusPenaltyAsync(int salaryRecordId, decimal? bonus, decimal? penalty);
    }
}
