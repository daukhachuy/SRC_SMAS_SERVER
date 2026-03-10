using SMAS_BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.DTOs.ReservationDTO
{
    public class ReservationListResponse
    {
        public int UserId { get; set; }

        public string Fullname { get; set; } = null!;

        public string? Phone { get; set; }

        public string? Email { get; set; }

        public int ReservationId { get; set; }

        public string? ReservationCode { get; set; }

        public DateOnly ReservationDate { get; set; }

        public TimeOnly ReservationTime { get; set; }

        public int NumberOfGuests { get; set; }

        public string? SpecialRequests { get; set; }

        public string? Status { get; set; }

        public DateTime? ConfirmedAt { get; set; }

        public DateTime? CancelledAt { get; set; }

        public string? CancellationReason { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public int? ConfirmedBy { get; set; }

        public string? ConfirmedByName { get; set; }   
    }
}
