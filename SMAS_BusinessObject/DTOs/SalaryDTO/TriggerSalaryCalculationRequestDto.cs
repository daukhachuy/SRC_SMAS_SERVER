using System.ComponentModel.DataAnnotations;

namespace SMAS_BusinessObject.DTOs.SalaryDTO
{
    public class TriggerSalaryCalculationRequestDto
    {
        [Range(1, 12, ErrorMessage = "Tháng phải từ 1 đến 12.")]
        public int Month { get; set; }

        [Range(2000, 2100, ErrorMessage = "Năm không hợp lệ.")]
        public int Year { get; set; }
    }
}
