using System;
using System.Collections.Generic;

namespace SMAS_BusinessObject.Models;

public partial class SalaryRecord
{
    public int SalaryRecordId { get; set; }

    public int UserId { get; set; }

    public int? TotalWorkingDay { get; set; }

    public decimal? TotalWorkingHours { get; set; }

    public int Month { get; set; }

    public int Year { get; set; }

    public decimal? SalaryPerHour { get; set; }

    public decimal? BaseSalary { get; set; }

    public decimal? Bonus { get; set; }

    public decimal? Penalty { get; set; }

    public decimal TotalSalary { get; set; }

    public string? PaymentStatus { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
