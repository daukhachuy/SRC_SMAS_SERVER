using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.Domain
{
    public class Staff
    {
        [Key]
        public int StaffId { get; set; }

        // FK → User
        [Required]
        public int UserId { get; set; }

        // FK → Shift
        public int? ShiftId { get; set; }

        public decimal? Salary { get; set; }

        [MaxLength(50)]
        public string? ExperienceLevel { get; set; } // Junior / Mid / Senior (nếu có)

        public DateTime? HireDate { get; set; }

        [MaxLength(50)]
        public string? Position { get; set; } // Waiter / Cashier / Kitchen

        [MaxLength(50)]
        public string? BankAccountNumber { get; set; }

        [MaxLength(100)]
        public string? BankName { get; set; }

        public double? Rating { get; set; }

        public bool IsWorking { get; set; } = true;

        public string? TaxId { get; set; }

    
        public virtual User? User { get; set; }

        public virtual WorkShift? Shift { get; set; }

        public virtual ICollection<Reservation>? ConfirmedReservations { get; set; }
    }
}
