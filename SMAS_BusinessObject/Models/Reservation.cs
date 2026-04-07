using System;
using System.Collections.Generic;

namespace SMAS_BusinessObject.Models;

public partial class Reservation
{
    public int ReservationId { get; set; }

    public int UserId { get; set; }

    public string? ReservationCode { get; set; }

    public DateOnly ReservationDate { get; set; }

    public TimeOnly ReservationTime { get; set; }

    public int NumberOfGuests { get; set; }

    public string? SpecialRequests { get; set; }

    public string? Status { get; set; }    // Pending = 1,Confirmed = 2,Cancelled = 3,Seated = 4 

    public int? ConfirmedBy { get; set; }

    public DateTime? ConfirmedAt { get; set; }

    public DateTime? CancelledAt { get; set; }

    public string? CancellationReason { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Staff? ConfirmedByNavigation { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual User User { get; set; } = null!;
}
