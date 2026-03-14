using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.DTOs.SalaryDTO
{
    public class SalaryMonthDto
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal TotalSalary { get; set; }
        public string? PaymentStatus { get; set; }  // XONG / CHƯA
        public DateTime? PaidAt { get; set; }       
    }

    
    public class SalaryLastSixMonthsResponseDto
    {
        public decimal AverageSalary { get; set; }                         
        public IEnumerable<SalaryMonthDto> Months { get; set; } = new List<SalaryMonthDto>();
    }
}
