using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.DTOs.WorkShiftDTO
{
    public class ScheduleWorkResponseDTO
    {
        public DateOnly WorkDate { get; set; }
        public string? ShiftName { get; set; } = null!;
        public string? AdditionalWork { get; set; }
        public string? Note { get; set; }
        public TimeOnly? StartTime { get; set; }
        public TimeOnly? EndTime { get; set; }
    }
}
