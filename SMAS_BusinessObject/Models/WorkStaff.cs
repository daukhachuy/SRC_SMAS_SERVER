using System;
using System.Collections.Generic;

namespace SMAS_BusinessObject.Models;

public partial class WorkStaff
{
    public int WorkStaffId { get; set; }

    public int UserId { get; set; }

    public int ShiftId { get; set; }

    public DateOnly WorkDay { get; set; }

    public bool? IsWorking { get; set; }

    public decimal? DailyTime { get; set; }

    public DateTime? CheckInTime { get; set; }

    public DateTime? CheckOutTime { get; set; }

    public string? Note { get; set; }

    public virtual WorkShift Shift { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
