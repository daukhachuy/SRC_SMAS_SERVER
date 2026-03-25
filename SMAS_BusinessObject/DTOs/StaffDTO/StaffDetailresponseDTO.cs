using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.DTOs.StaffDTO
{
    public class StaffDetailresponseDTO
    {
        public int UserId { get; set; }

        public string Fullname { get; set; } = null!;

        public string? Phone { get; set; }

        public string? Email { get; set; }

        public string? Address { get; set; }

        public decimal? Salary { get; set; }

        public DateOnly HireDate { get; set; }

        public string? Position { get; set; }

        public string? BankAccountNumber { get; set; }

        public string? BankName { get; set; }

        public string? TaxId { get; set; }
    }

    public class StaffDetailRequestDTO
    {
        [Required(ErrorMessage = "UserId không được để trống")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Tên không được để trống")]
        [StringLength(100, ErrorMessage = "Tên tối đa 100 ký tự")]
        public string Fullname { get; set; } = null!;
        [Required(ErrorMessage = "Email không được để trống")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [StringLength(15, ErrorMessage = "Số điện thoại tối đa 15 ký tự")]
        public string? Phone { get; set; }

        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(100, ErrorMessage = "Email tối đa 100 ký tự")]
        public string? Email { get; set; }

        [StringLength(255, ErrorMessage = "Địa chỉ tối đa 255 ký tự")]
        public string? Address { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Lương phải >= 0")]
        public decimal? Salary { get; set; }
        [Required(ErrorMessage = "Vị trí không được để trống")]
        [StringLength(50, ErrorMessage = "Vị trí tối đa 50 ký tự")]
        public string? Position { get; set; }

        [StringLength(50, ErrorMessage = "Số tài khoản tối đa 50 ký tự")]
        public string? BankAccountNumber { get; set; }

        [StringLength(100, ErrorMessage = "Tên ngân hàng tối đa 100 ký tự")]
        public string? BankName { get; set; }

        [StringLength(20, ErrorMessage = "Mã số thuế tối đa 20 ký tự")]
        public string? TaxId { get; set; }
    }
}
