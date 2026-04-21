using System.ComponentModel.DataAnnotations;

namespace SMAS_BusinessObject.DTOs.SalaryDTO
{
    public class AdjustBonusPenaltyRequestDto
    {
        [Range(0, double.MaxValue, ErrorMessage = "Bonus không được âm.")]
        public decimal? Bonus { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Penalty không được âm.")]
        public decimal? Penalty { get; set; }
    }
}
