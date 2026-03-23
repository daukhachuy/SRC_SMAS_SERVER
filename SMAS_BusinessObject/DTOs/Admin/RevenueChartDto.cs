using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.DTOs.Admin
{
    public class RevenueChartDto
    {
        public List<RevenueChartItemDto> Data { get; set; } = new();
    }

    public class RevenueChartItemDto
    {
        public string Month { get; set; } = null!;

        /// <summary>Tổng doanh thu từ Order.TotalAmount (OrderStatus = Completed)</summary>
        public decimal Revenue { get; set; }

        /// <summary>Tổng chi phí nhập kho từ Transaction.TotalAmount (TransactionType = Import)</summary>
        public decimal Cost { get; set; }
    }

    public class OrderStructureDto
    {
        ///Số đơn ăn tại chỗ
        public int DineIn { get; set; }

        ///>Số đơn mang về
        public int TakeAway { get; set; }

        ///Số đơn giao hàng
        public int Delivery { get; set; }

        ///Số đơn sự kiện (BookEventId != null)
        public int Event { get; set; }

        ///Tổng số đơn
        public int Total => DineIn + TakeAway + Delivery + Event;
    }

    public class WarehouseTransactionDto
    {
        public string? TransactionCode { get; set; }
        public string? SupplierName { get; set; }
        public decimal TotalAmount { get; set; }
        public string? PaymentStatus { get; set; }
        public DateTime TransactionDate { get; set; }
    }

}
