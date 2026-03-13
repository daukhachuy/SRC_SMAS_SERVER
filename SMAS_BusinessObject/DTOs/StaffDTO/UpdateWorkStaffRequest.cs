using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.DTOs.StaffDTO
{
    public class UpdateWorkStaffRequestDto
    {
        public int? ReplaceUserId { get; set; }      
        public DateTime? CheckInTime { get; set; }  
        public DateTime? CheckOutTime { get; set; } 
        public bool? IsWorking { get; set; }         
        public string? Note { get; set; }
    }
}
