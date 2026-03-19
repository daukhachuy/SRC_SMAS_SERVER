using System.ComponentModel.DataAnnotations;

namespace SMAS_BusinessObject.DTOs.OrderDTO
{
    public class CreateOrderByReservationRequest
    {
        [Required]
        public string ReservationCode { get; set; } = string.Empty;

        [Required]
        public int TableId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int NumberOfGuests { get; set; }

        public string? Note { get; set; }

        public List<OrderItemRequest>? OrderItems { get; set; }
    }

    public class CreateOrderByContactRequest
    {
        public string? Phone { get; set; }
        public string? Email { get; set; }

        [Required]
        public int TableId { get; set; }

        [Required]
        public string OrderType { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue)]
        public int NumberOfGuests { get; set; }

        public string? Note { get; set; }

        public List<OrderItemRequest>? OrderItems { get; set; }
    }

    public class CreateGuestOrderRequest
    {
        [Required]
        public int TableId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int NumberOfGuests { get; set; }

        public string? Note { get; set; }

        public List<OrderItemRequest>? OrderItems { get; set; }
    }

    public class OrderItemRequest
    {
        public int? FoodId { get; set; }
        public int? BuffetId { get; set; }
        public int? ComboId { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        public string? Note { get; set; }
    }

    public class CreateInHouseOrderResponse
    {
        public int OrderId { get; set; }
        public string? OrderCode { get; set; }
        public string? OrderStatus { get; set; }
        public string? OrderType { get; set; }
        public int TableId { get; set; }
        public int NumberOfGuests { get; set; }
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
