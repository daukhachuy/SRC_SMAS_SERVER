using System;
using System.Collections.Generic;

namespace SMAS_BusinessObject.Models;

public partial class WorkShift
{
    public int ShiftId { get; set; }

    public string? ShiftName { get; set; }

    public TimeOnly? StartTime { get; set; }

    public TimeOnly? EndTime { get; set; }

    public string? AdditionalWork { get; set; }

    public string? TypeStaff { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<WorkStaff> WorkStaffs { get; set; } = new List<WorkStaff>();
}
