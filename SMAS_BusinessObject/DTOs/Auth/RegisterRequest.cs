using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.DTOs.Auth
{
    public class RegisterRequest
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Fullname { get; set; } = null!;
        public string? Gender { get; set; }
        public DateOnly? Dob { get; set; }                  
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? Avatar { get; set; }                
    }
}

