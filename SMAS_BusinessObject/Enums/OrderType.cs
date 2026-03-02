using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.Enums
{
    public enum OrderType
    {
        None,           // Giá trị mặc định khi chưa xác định loại đơn hàng nào
        DineIn,         // Khách hàng ăn tại nhà hàng
        TakeAway,       // Khách hàng đặt món mang đi
        Delivery,       // Khách hàng đặt món giao hàng tận nơi
        EventBooking    // Khách hàng đặt chỗ cho sự kiện hoặc tiệc tùng tại nhà hàng
    }
}
