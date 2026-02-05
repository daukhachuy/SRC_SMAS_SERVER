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
        MSG_999 // Dữ liệu đầu vào rỗng 
    }
}
