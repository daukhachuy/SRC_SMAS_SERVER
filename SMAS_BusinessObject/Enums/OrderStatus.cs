using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.Enums
{
    public enum OrderStatus
    {
        Unknown = 0,
        Pending = 1,       // Đang chờ xử lý
        Confirmed = 2,     // Đã xác nhận
        Preparing = 3,     // Đang thưởng thức
        Completed = 4,     // Đã hoàn thành
        Cancelled = 5,     // Đã hủy
        Paid      = 6
    }
}
