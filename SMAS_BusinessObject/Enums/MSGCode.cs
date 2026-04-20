using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.Enums
{
    public enum MSGCode
    {
        MSG_001, // Email không tồn tại 
        MSG_002, // Sai mật khẩu 
        MSG_003, // Đăng nhập thành công 
        MSG_004, // Tài khoản đã được đăng ký 
        MSG_005, // Email đã tồn tại
        MSG_006, // OTP đã gửi
        MSG_007, // OTP sai hoặc hết hạn
        MSG_008, // Đổi mật khẩu thành công
        MSG_009, // Xác minh OTP thành công
        MSG_010, // Cập nhật profile thành công
        MSG_011, // Bạn đã đặt chỗ vào ngày giờ này rồi
        MSG_013, // Mã giảm giá không tồn tại
        MSG_014, // Không có mã giảm giá nào
        MSG_015, // Không có sự kiện nào
        MSG_016, // Không có phản hồi nào
        MSG_017, // Không có món ăn nào
        MSG_018, // Không có món ăn nào đang giảm giá
        MSG_019, // Không có món ăn nào được bán chạy
        MSG_020, // Không tìm thấy buffer
        MSG_021,
        MSG_022, // Không có dịch vụ nào đang hoạt động
        MSG_023,
        MSG_024,
        MSG_025, // Không có combo nào đang hoạt động
        MSG_026, // Không có combo nào phù hợp với tiêu chí lọc
        MSG_027,
        MSG_028,
        MSG_029,// Vui lòng nhập mật khẩu hiện tại
        MSG_030,// Tài khoản Google không hỗ trợ đổi mật khẩu
        MSG_031,// Mật khẩu hiện tại không đúng
        MSG_999 // Dữ liệu đầu vào rỗng 
    }
}
