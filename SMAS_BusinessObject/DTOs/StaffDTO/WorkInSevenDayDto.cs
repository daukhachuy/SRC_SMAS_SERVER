using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.DTOs.StaffDTO
{
    public class StaffSlotDto
    {
        public int WorkStaffId { get; set; }
        public int UserId { get; set; }
        public string FullName { get; set; } = null!;
        public string? AvatarUrl { get; set; }
        public string? Position { get; set; }
    }

  
    public class ScheduleSlotDto
    {
        public DateOnly WorkDay { get; set; }       
        public IEnumerable<StaffSlotDto> Staffs { get; set; } = new List<StaffSlotDto>();
    }

    
    public class ShiftRowDto
    {
        public int ShiftId { get; set; }
        public string? ShiftName { get; set; }       // Sáng / Chiều / Tối
        public TimeOnly? StartTime { get; set; }     
        public TimeOnly? EndTime { get; set; }      
        public IEnumerable<ScheduleSlotDto> Days { get; set; } = new List<ScheduleSlotDto>();
    }

    // Response tổng: danh sách 7 ngày + các hàng ca
    public class WorkNextSevenDayResponseDto
    {
        public IEnumerable<DateOnly> DateRange { get; set; } = new List<DateOnly>(); 
        public IEnumerable<ShiftRowDto> Shifts { get; set; } = new List<ShiftRowDto>();
    }
}
