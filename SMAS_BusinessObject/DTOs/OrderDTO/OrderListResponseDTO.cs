using SMAS_BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.DTOs.OrderDTO
{
    public class OrderListResponseDTO
    {
        public int OrderId { get; set; }
        public string? OrderCode { get; set; }
        public string? OrderType { get; set; }
        public string? OrderStatus { get; set; }

        public int? NumberOfGuests { get; set; }

        public decimal? SubTotal { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? TaxAmount { get; set; }
        public decimal? DeliveryPrice { get; set; }
        public decimal TotalAmount { get; set; }

        public string? Note { get; set; }
        public List<TableInfoDto> Tables { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public DateTime? ClosedAt { get; set; }

        public UserInfoDto Customer { get; set; } = null!;
        public StaffInfoDto? ServedBy { get; set; }

        public DeliveryDto? Delivery { get; set; }

        public List<OrderItemDetailDto> Items { get; set; } = new();
        public List<PaymentDto> Payments { get; set; } = new();

    }

    public class PaymentDto
    {
        public int PaymentId { get; set; }
        public string PaymentMethod { get; set; } = null!;
        public string? PaymentStatus { get; set; }
        public decimal Amount { get; set; }
        public DateTime? PaidAt { get; set; }
    }

    public class UserInfoDto
    {
        public int UserId { get; set; }
        public string Fullname { get; set; } = null!;
        public string? Phone { get; set; }
        public string? Email { get; set; }
    }

    public class StaffInfoDto
    {
        public int UserId { get; set; }
        public string Fullname { get; set; } = null!;
    }
    public class TableInfoDto
    {
        public int TableId { get; set; }
        public string TableName { get; set; } = string.Empty;
        public bool IsMainTable { get; set; }
    }
    public class DeliveryDto
    {
        public int DeliveryId { get; set; }
        public string RecipientName { get; set; } = null!;
        public string RecipientPhone { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string? DeliveryStatus { get; set; }
        public decimal? DeliveryFee { get; set; }
        public DateTime? EstimatedDeliveryTime { get; set; }
        public DateTime? ActualDeliveryTime { get; set; }
    }
    public class OrderItemDetailDto
    {
        public int OrderItemId { get; set; }
        public string ItemType { get; set; } = null!; 
        public string ItemName { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal? Subtotal { get; set; }

        public string? Status { get; set; }
        public DateTime? OpeningTime { get; set; }
        public DateTime? ServedTime { get; set; }
    }
}
