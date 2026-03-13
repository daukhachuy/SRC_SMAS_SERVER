using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.DTOs.StaffDTO
{
    public class WorkHistoryDetailDto
    {
        public DateOnly WorkDay { get; set; }
        public string? ShiftName { get; set; }      
        public TimeOnly? CheckInTime { get; set; }  
        public TimeOnly? CheckOutTime { get; set; } 
        public decimal? DailyTime { get; set; }     
        public string Status { get; set; } = null!; // Đúng giờ / Muộn(Xp) / Vắng
    }

    public class WorkHistoryResponseDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = null!;
        public string? AvatarUrl { get; set; }

        public string? Position { get; set; }       

        // Thống kê tháng
        public int TotalShifts { get; set; }        
        public decimal TotalHours { get; set; }     
        public int TotalAbsent { get; set; }        

        // Chi tiết từng ngày
        public IEnumerable<WorkHistoryDetailDto> Details { get; set; } = new List<WorkHistoryDetailDto>();
    }
}
