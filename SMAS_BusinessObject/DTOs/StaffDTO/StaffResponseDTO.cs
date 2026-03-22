using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.DTOs.StaffDTO
{
    public class StaffResponseDTO
    {
        public int UserId { get; set; }
        public string Fullname { get; set; } = null!;
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Avatar { get; set; }
        public bool? IsDeleted { get; set; }
        public DateOnly HireDate { get; set; }
        public string? Position { get; set; }
    }
}
