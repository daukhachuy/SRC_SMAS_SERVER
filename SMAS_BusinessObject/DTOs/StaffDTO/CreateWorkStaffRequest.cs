using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.DTOs.StaffDTO
{
    public class CreateWorkStaffRequestDto
    {
        public int UserId { get; set; }     
        public int ShiftId { get; set; }     
        public DateOnly WorkDay { get; set; } 
        public string? Note { get; set; }   
    }

}
