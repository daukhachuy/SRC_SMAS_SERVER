using SMAS_BusinessObject.DTOs.OrderDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.DTOs.PDFDTO
{
    public class PdfInvoiceDTO
    {

        public string? OrderCode { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerPhone { get; set; }
        public string? OrderType { get; set; }
        public int? NumberOfGuests { get; set; }
        public decimal? SubTotal { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? TaxAmount { get; set; }
        public decimal? DeliveryPrice { get; set; }
        public decimal TotalAmount { get; set; }
        public List<OrderItemInvoice> Items { get; set; } = new();
        public string PaymentMethod { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime? ClosedAt { get; set; }
    }
    public class OrderItemInvoice
    {
        public string ItemName { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal? Subtotal { get; set; }
    }
}
