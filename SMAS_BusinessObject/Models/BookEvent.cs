using System;
using System.Collections.Generic;

namespace SMAS_BusinessObject.Models;

public partial class BookEvent
{
    public int BookEventId { get; set; }

    public string? BookingCode { get; set; }

    public int EventId { get; set; }

    public int CustomerId { get; set; }

    public int NumberOfGuests { get; set; }

    public DateOnly ReservationDate { get; set; }

    public TimeOnly ReservationTime { get; set; }

    public bool? IsContract { get; set; }

    public int? ContractId { get; set; }

    public int? ConfirmedBy { get; set; }

    public DateTime? ConfirmedAt { get; set; }

    public string? Note { get; set; }

    public string? Status { get; set; }

    public decimal? TotalAmount { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<BookEventService> BookEventServices { get; set; } = new List<BookEventService>();

    public virtual Staff? ConfirmedByNavigation { get; set; }

    public virtual Contract? Contract { get; set; }

    public virtual ICollection<Contract> Contracts { get; set; } = new List<Contract>();

    public virtual User Customer { get; set; } = null!;

    public virtual Event Event { get; set; } = null!;

    public virtual ICollection<EventFood> EventFoods { get; set; } = new List<EventFood>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
