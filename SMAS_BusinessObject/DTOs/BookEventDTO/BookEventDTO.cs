using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.DTOs.BookEventDTO
{
    public class BookEventListResponseDTO
    {
        public int BookEventId { get; set; }
        public string? BookingCode { get; set; }
        public string? Status { get; set; }
        public int NumberOfGuests { get; set; }
        public DateOnly ReservationDate { get; set; }
        public TimeOnly ReservationTime { get; set; }
        public bool? IsContract { get; set; }
        public decimal? TotalAmount { get; set; }
        public string? Note { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Confirmed by
        public StaffBookEventDto? ConfirmedBy { get; set; }
        public DateTime? ConfirmedAt { get; set; }

        // Customer
        public UserBookEventDto Customer { get; set; } = null!;

        // Event
        public EventBookEventDto Event { get; set; } = null!;

        // Contract
        public ContractBookEventDto? Contract { get; set; }

        // Services
        public List<BookEventServiceDto> Services { get; set; } = new();

        // Foods (món ăn theo sự kiện - EventFood)
        public List<BookEventFoodDto> Foods { get; set; } = new();
    }

    public class BookEventFoodDto
    {
        public int FoodId { get; set; }
        public string? FoodName { get; set; }
        public int Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? TotalPrice => (UnitPrice.HasValue && Quantity > 0) ? UnitPrice.Value * Quantity : null;
        public string? Note { get; set; }
    }

    public class UserBookEventDto
    {
        public int UserId { get; set; }
        public string Fullname { get; set; } = null!;
        public string? Phone { get; set; }
        public string? Email { get; set; }
    }

    public class EventBookEventDto
    {
        public int EventId { get; set; }
        public string Title { get; set; } = null!;
        public string? EventType { get; set; }
        public string? Image { get; set; }
        public decimal? BasePrice { get; set; }
    }

    public class StaffBookEventDto
    {
        public int UserId { get; set; }
        public string Fullname { get; set; } = null!;
    }

    public class ContractBookEventDto
    {
        public int ContractId { get; set; }
        public string? ContractCode { get; set; }
        public string? Status { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal? DepositAmount { get; set; }
        public decimal? RemainingAmount { get; set; }
    }

    public class BookEventServiceDto
    {
        public int ServiceId { get; set; }
        public string? ServiceName { get; set; }
        public string? Unit { get; set; }
        public int? Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? TotalPrice => (UnitPrice.HasValue && Quantity.HasValue)
            ? UnitPrice.Value * Quantity.Value
            : null;
        public string? Note { get; set; }
    }
}
