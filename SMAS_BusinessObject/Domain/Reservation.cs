using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.Domain
{
    public class Reservation
    {
        [Key]
        public int ReservationId { get; set; }

        // FK → Users
        [Required]
        public int UserId { get; set; }

        public TimeSpan ReservationTime { get; set; }

        public DateTime ReservationDate { get; set; }

        public int NumberOfGuests { get; set; }

        public string? Note { get; set; }

        // FK → Staff
        public int? ConfirmedByStaffId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdateAt { get; set; }

        public string? Status { get; set; }

        public string? ReservationQrCode { get; set; }

        // FK → Event
        public int? EventId { get; set; }

        // Navigation properties
        public virtual User? User { get; set; }

        public virtual Staff? ConfirmedByStaff { get; set; }

        public virtual Event? Event { get; set; }
    }
}
