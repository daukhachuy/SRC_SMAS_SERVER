using SMAS_BusinessObject.DTOs.SalaryDTO;
using SMAS_BusinessObject.Models;
using SMAS_Repositories.SalaryRepository;

namespace SMAS_Services.SalaryService
{
    public class SalaryRecordService : ISalaryRecordService
    {
        private readonly ISalaryRecordRepository _salaryRecordRepository;

        public SalaryRecordService(ISalaryRecordRepository salaryRecordRepository)
        {
            _salaryRecordRepository = salaryRecordRepository;
        }

        public async Task<SalaryLastSixMonthsResponseDto> GetSalaryLastSixMonthsAsync(int userId)
            => await _salaryRecordRepository.GetSalaryLastSixMonthsAsync(userId);

        public async Task<MonthlySalaryDetailResponseDto?> GetCurrentMonthlySalaryDetailAsync(int userId)
            => await _salaryRecordRepository.GetCurrentMonthlySalaryDetailAsync(userId);

        public async Task<int> CalculateAndSaveMonthlySalaryAsync(
            int month, int year,
            decimal penaltyPerLateMinute,
            decimal fullMonthBonus,
            decimal defaultSalaryPerHour)
        {
            if (await _salaryRecordRepository.ExistsAsync(month, year))
                return 0;

            var staffList = await _salaryRecordRepository.GetAllActiveStaffAsync();
            if (staffList.Count == 0)
                return 0;

            var salaryRecords = new List<SalaryRecord>();

            foreach (var staff in staffList)
            {
                var workEntries = await _salaryRecordRepository.GetWorkStaffByMonthAsync(staff.UserId, month, year);
                if (workEntries.Count == 0)
                    continue;

                var baseSalary = staff.Salary ?? 0;
                var salaryPerHour = defaultSalaryPerHour;

                int totalWorkingDays = 0;
                decimal totalWorkingHours = 0;
                decimal totalLateMinutes = 0;
                decimal totalOvertimeMinutes = 0;
                int scheduledShifts = workEntries.Count;
                int attendedShifts = 0;

                foreach (var entry in workEntries)
                {
                    if (entry.CheckInTime == null || entry.Shift == null)
                        continue;

                    attendedShifts++;
                    totalWorkingDays++;

                    if (entry.CheckInTime != null && entry.CheckOutTime != null)
                    {
                        var workedHours = (decimal)(entry.CheckOutTime.Value - entry.CheckInTime.Value).TotalHours;
                        totalWorkingHours += Math.Max(0, workedHours);
                    }

                    var shiftStartDateTime = entry.WorkDay.ToDateTime(entry.Shift.StartTime ?? TimeOnly.MinValue);
                    if (entry.CheckInTime.Value > shiftStartDateTime)
                    {
                        var late = (decimal)(entry.CheckInTime.Value - shiftStartDateTime).TotalMinutes;
                        totalLateMinutes += late;
                    }

                    if (entry.CheckOutTime != null && entry.Shift.EndTime != null)
                    {
                        var shiftEndDateTime = entry.WorkDay.ToDateTime(entry.Shift.EndTime.Value);
                        if (entry.CheckOutTime.Value > shiftEndDateTime)
                        {
                            var overtime = (decimal)(entry.CheckOutTime.Value - shiftEndDateTime).TotalMinutes;
                            totalOvertimeMinutes += overtime;
                        }
                    }
                }

                var penalty = totalLateMinutes * penaltyPerLateMinute;
                var overtimePay = (totalOvertimeMinutes / 60m) * salaryPerHour;
                var bonus = (attendedShifts >= scheduledShifts && scheduledShifts > 0) ? fullMonthBonus : 0;
                var totalSalary = Math.Max(0, baseSalary + overtimePay + bonus - penalty);

                salaryRecords.Add(new SalaryRecord
                {
                    UserId = staff.UserId,
                    Month = month,
                    Year = year,
                    TotalWorkingDay = totalWorkingDays,
                    TotalWorkingHours = Math.Round(totalWorkingHours, 2),
                    SalaryPerHour = salaryPerHour,
                    BaseSalary = baseSalary,
                    Bonus = bonus,
                    Penalty = penalty,
                    TotalSalary = totalSalary,
                    PaymentStatus = "Pending",
                    CreatedAt = DateTime.Now
                });
            }

            if (salaryRecords.Count > 0)
                await _salaryRecordRepository.CreateBatchAsync(salaryRecords);

            return salaryRecords.Count;
        }

        public async Task<List<SalaryRecordListItemDto>> GetAllSalaryByMonthAsync(int month, int year)
            => await _salaryRecordRepository.GetAllByMonthAsync(month, year);

        public async Task<MonthlySalaryDetailResponseDto?> GetSalaryDetailByUserAndMonthAsync(int userId, int month, int year)
            => await _salaryRecordRepository.GetByUserAndMonthAsync(userId, month, year);

        public async Task<(bool Success, string Message)> AdjustBonusPenaltyAsync(int salaryRecordId, decimal? bonus, decimal? penalty)
        {
            var record = await _salaryRecordRepository.GetByIdAsync(salaryRecordId);
            if (record == null)
                return (false, "Không tìm thấy bản ghi lương.");

            if (record.PaymentStatus == "Paid")
                return (false, "Không thể chỉnh sửa bản ghi đã trả lương.");

            if (bonus.HasValue) record.Bonus = bonus.Value;
            if (penalty.HasValue) record.Penalty = penalty.Value;

            record.TotalSalary = Math.Max(0,
                (record.BaseSalary ?? 0) + (record.Bonus ?? 0) - (record.Penalty ?? 0));

            await _salaryRecordRepository.UpdateAsync(record);
            return (true, "Cập nhật Bonus/Penalty thành công.");
        }

    }
}
