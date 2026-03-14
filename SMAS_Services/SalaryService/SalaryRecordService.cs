using SMAS_BusinessObject.DTOs.SalaryDTO;
using SMAS_Repositories.SalaryRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }

}
