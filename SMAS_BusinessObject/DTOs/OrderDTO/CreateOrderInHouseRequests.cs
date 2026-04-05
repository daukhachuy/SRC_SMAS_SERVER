using System.ComponentModel.DataAnnotations;

namespace SMAS_BusinessObject.DTOs.OrderDTO
{
    // Shared: Request DTO for order items (used by all 3 APIs)
    public class OrderItemRequest
    {
        public int? FoodId { get; set; }
        public int? BuffetId { get; set; }
        public int? ComboId { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        public string? Note { get; set; }
    }

    public class CreateOrderByReservationRequest
    {
        [Required]
        public string ReservationCode { get; set; } = string.Empty;

        public string? Note { get; set; }
    }

    public class CreateOrderByContactRequest
    {
        public string? Phone { get; set; }
        public string? Email { get; set; }

        /// <summary>DineIn hoặc TakeAway</summary>
        [Required]
        public string OrderType { get; set; } = string.Empty;

        /// <summary>Số khách do waiter nhập.</summary>
        [Required]
        [Range(1, int.MaxValue)]
        public int NumberOfGuests { get; set; }

        public string? Note { get; set; }
    }

    // API 3: guest order (no reservation and no phone/email)
    public class CreateGuestOrderRequest
    {
        /// <summary>DineIn hoặc TakeAway</summary>
        [Required]
        public string OrderType { get; set; } = string.Empty;

        /// <summary>Số khách do waiter nhập.</summary>
        [Required]
        [Range(1, int.MaxValue)]
        public int NumberOfGuests { get; set; }

        public string? Note { get; set; }
    }

    // API 0: lookup thông tin khách trước khi tạo order
    public class OrderLookupRequestDto
    {
        [Required]
        public string Type { get; set; } = string.Empty; // "Reservation" | "Member" | "Guest"

        public string? Keyword { get; set; }
    }

    public class OrderLookupResponseDto
    {
        public string Type { get; set; } = string.Empty;

        public int? UserId { get; set; }
        public string? FullName { get; set; }
        public string? Phone { get; set; }

        public string? ReservationCode { get; set; }
        public int? ReservationId { get; set; }
        public int? NumberOfGuests { get; set; }
        public DateOnly? ReservationDate { get; set; }
        public TimeOnly? ReservationTime { get; set; }

        // Theo spec: Reservation => "DineIn"; Member/Guest => null (waiter tự chọn)
        public string? OrderType { get; set; }
    }

    // Shared: Response DTO (201 Created) for all 3 APIs
    public class CreateInHouseOrderResponse
    {
        public int OrderId { get; set; }
        public string? OrderCode { get; set; }
        public string? OrderStatus { get; set; }
        public string? OrderType { get; set; }
        /// <summary>Đặt bàn: theo reservation; contact/guest: theo waiter nhập.</summary>
        public int? NumberOfGuests { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TotalAmount { get; set; }
        public List<CreateInHouseOrderItemResponse> OrderItems { get; set; } = new();
        public DateTime CreatedAt { get; set; }
    }

    public class CreateInHouseOrderItemResponse
    {
        public int OrderItemId { get; set; }
        public int? FoodId { get; set; }
        public int? BuffetId { get; set; }
        public int? ComboId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal { get; set; }
        public string? Status { get; set; }
        public string? Note { get; set; }
    }
}
