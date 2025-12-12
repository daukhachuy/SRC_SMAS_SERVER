using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.Domain
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Fullname { get; set; }

        [MaxLength(10)]
        public string? Gender { get; set; }   // Male / Female / Other

        public DateTime? DOB { get; set; }    

        [MaxLength(20)]
        public string? Phone { get; set; }

        [MaxLength(150)]
        public string? Email { get; set; }

        public string? Address { get; set; }

        public string? Avatar { get; set; }   // URL hoặc path ảnh

        public DateTime CreatedAT { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAT { get; set; }
     
        [Required]
        [MaxLength(20)]
        public string Role { get; set; }   // Admin, Staff, Customer

        [Required]
        public string PassWord { get; set; }

        public bool IsActive { get; set; } = true;

        public virtual ICollection<Reservation>? Reservations { get; set; }
    }
}

