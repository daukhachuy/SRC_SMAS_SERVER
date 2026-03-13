using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.DTOs.StaffDTO
{
    public class StaffWorkingTodayDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = null!;
        public string? AvatarUrl { get; set; }  
        public string? Position { get; set; }    
        public string? Note{ get; set; }    
        public DateTime? CheckInTime { get; set; } 
    }
}
