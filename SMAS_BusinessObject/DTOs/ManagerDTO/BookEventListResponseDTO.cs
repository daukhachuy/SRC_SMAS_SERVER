using System;

namespace SMAS_BusinessObject.DTOs.ManagerDTO;

/// <summary>
/// DTO danh sách đặt sự kiện (BookEvent) cho Manager
/// </summary>
public class BookEventListResponseDTO
{
    public int BookEventId { get; set; }
    public string? BookingCode { get; set; }
    public int EventId { get; set; }
    public string? EventTitle { get; set; }
    public int CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerPhone { get; set; }
    public string? CustomerEmail { get; set; }
    public int NumberOfGuests { get; set; }
    public DateOnly ReservationDate { get; set; }
    public TimeOnly ReservationTime { get; set; }
    public string? Status { get; set; }
    public decimal? TotalAmount { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? ConfirmedAt { get; set; }
    public int? ConfirmedBy { get; set; }
    public string? ConfirmedByName { get; set; }
}
