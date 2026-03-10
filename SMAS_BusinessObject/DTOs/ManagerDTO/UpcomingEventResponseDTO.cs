using System;

namespace SMAS_BusinessObject.DTOs.ManagerDTO;

/// <summary>
/// DTO sự kiện sắp tới (BookEvent có ReservationDate >= hôm nay)
/// </summary>
public class UpcomingEventResponseDTO
{
    public int BookEventId { get; set; }
    public string? BookingCode { get; set; }
    public int EventId { get; set; }
    public string? EventTitle { get; set; }
    public string? EventType { get; set; }
    public int CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerPhone { get; set; }
    public int NumberOfGuests { get; set; }
    public DateOnly ReservationDate { get; set; }
    public TimeOnly ReservationTime { get; set; }
    public string? Status { get; set; }
    public decimal? TotalAmount { get; set; }
    public DateTime? CreatedAt { get; set; }
}
