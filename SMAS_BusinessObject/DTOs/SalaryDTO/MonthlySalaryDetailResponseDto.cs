using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.DTOs.SalaryDTO
{
    public class MonthlySalaryDetailResponseDto
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public string? PaymentStatus { get; set; }   
     
        public string PaymentPeriod => $"01/{Month:D2} - {DateTime.DaysInMonth(Year, Month):D2}/{Month:D2}";

    
        public decimal? BaseSalary { get; set; }      
        public string? ExperienceLevel { get; set; }
        public decimal? Bonus { get; set; }        
        public decimal? Penalty { get; set; }        

     
        public int? TotalWorkingDay { get; set; }
        public decimal? TotalWorkingHours { get; set; }
        public decimal TotalSalary { get; set; }      
    }

}
