using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.DTOs.Auth
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(100, ErrorMessage = "Email không được vượt quá 100 ký tự")]
        public string Email { get; set; } = null!;
        [Required(ErrorMessage = "Password không được để trống")]
        [StringLength(50, MinimumLength = 6,
        ErrorMessage = "Password phải từ 6 đến 50 ký tự")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d@$!%*#?&]+$",
        ErrorMessage = "Password phải chứa ít nhất 1 chữ và 1 số")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;
    }
}
