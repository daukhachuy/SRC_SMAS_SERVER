using SMAS_BusinessObject.DTOs.SalaryDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Services.SalaryService
{
    public interface ISalaryRecordService
    {
        Task<SalaryLastSixMonthsResponseDto> GetSalaryLastSixMonthsAsync(int userId);
        Task<MonthlySalaryDetailResponseDto?> GetCurrentMonthlySalaryDetailAsync(int userId);
    }

}
