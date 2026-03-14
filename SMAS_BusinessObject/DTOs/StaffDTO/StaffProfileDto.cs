using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Repositories.StaffRepository
{
    public class StaffProfileDto
    {
        public string FullName { get; set; } = null!;
        public string? AvatarUrl { get; set; }
        public string? Position { get; set; }        
        public string? ExperienceLevel { get; set; }
        public string? Phone { get; set; }        
        public DateOnly? HireDate { get; set; }     
        public string? Gender { get; set; }
        public DateOnly? Dob { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }     
        public string? BankAccountNumber { get; set; }
        public string? BankName { get; set; }

        public string? Role { get; set; }            
        public DateOnly? HireDateReadOnly => HireDate; 
        public string? TaxId { get; set; }           
    }

    // POST - Cập nhật hồ sơ (không có Role, HireDate, TaxId)
    public class UpdateProfileStaffRequestDto
    {
        public string? Fullname { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Gender { get; set; }
        public DateOnly? Dob { get; set; }
        public string? Address { get; set; }
        public string? AvatarUrl { get; set; }
        public string? BankAccountNumber { get; set; }
        public string? BankName { get; set; }
    }

}
