namespace SMAS_BusinessObject.DTOs.SalaryDTO
{
    public class SalaryRecordListItemDto
    {
        public int SalaryRecordId { get; set; }
        public int UserId { get; set; }
        public string? Fullname { get; set; }
        public string? Position { get; set; }
        public int? TotalWorkingDay { get; set; }
        public decimal? TotalWorkingHours { get; set; }
        public decimal? BaseSalary { get; set; }
        public decimal? Bonus { get; set; }
        public decimal? Penalty { get; set; }
        public decimal TotalSalary { get; set; }
        public string? PaymentStatus { get; set; }
    }
}
