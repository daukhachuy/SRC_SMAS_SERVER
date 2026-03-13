using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.DTOs.StaffDTO
{
    public class WorkShiftDto
    {
        public int ShiftId { get; set; }
        public string? ShiftName { get; set; }
        public TimeOnly? StartTime { get; set; }
        public TimeOnly? EndTime { get; set; }
        public string? TypeStaff { get; set; }   // Waiter / Kitchen
        public string? AdditionalWork { get; set; }
    }
}
