using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.DTOs.StaffDTO
{
    public class FilterStaffByPositionDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = null!;
        public string? AvatarUrl { get; set; }
        public string? Position { get; set; }
        public string? Phone { get; set; }
        public DateOnly? HireDate { get; set; }
        public decimal? Rating { get; set; }
    }


}
