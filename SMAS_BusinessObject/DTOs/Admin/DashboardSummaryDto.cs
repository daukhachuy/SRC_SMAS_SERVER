using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.DTOs.Admin
{
    public class DashboardSummaryDto
    {
        /// <summary>Tổng doanh thu từ các đơn hàng hoàn thành trong tháng hiện tại</summary>
        public decimal TotalRevenue { get; set; }

        /// <summary>Chi phí nhập kho (Transaction loại Import) trong tháng hiện tại</summary>
        public decimal WarehouseCost { get; set; }

        /// <summary>Số hợp đồng mới tạo trong tháng hiện tại</summary>
        public int NewContracts { get; set; }

        /// <summary>Số khách hàng mới đăng ký trong tháng hiện tại</summary>
        public int NewCustomers { get; set; }
    }
}
