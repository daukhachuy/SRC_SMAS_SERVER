using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.DTOs.StaffDTO
{
    public class CreateNewStaffByUseridResquestDTO
    {
        [Required(ErrorMessage = "UserId không được để trống")]
        public int UserId { get; set; }
        [Required(ErrorMessage = "Lương  không được để trống")]
        [Range(1, double.MaxValue, ErrorMessage = "Salary phải >= 0")]
        public decimal? Salary { get; set; }
        [Required(ErrorMessage = "Vị trí làm việc không được để trống")]
        [StringLength(100, ErrorMessage = "Position tối đa 100 ký tự")]
        public string? Position { get; set; }

        [StringLength(50, ErrorMessage = "Số tài khoản tối đa 50 ký tự")]
        public string? BankAccountNumber { get; set; }

        [StringLength(100, ErrorMessage = "Tên ngân hàng tối đa 100 ký tự")]
        public string? BankName { get; set; }
        [Required(ErrorMessage = "Tax không được để trống")]
        [StringLength(20, ErrorMessage = "TaxId tối đa 20 ký tự")]
        public string? TaxId { get; set; }
    }

    public class CreateNewStaffRequestDTO
    {
        [Required(ErrorMessage = "Fullname không được để trống")]
        [StringLength(150, ErrorMessage = "Fullname tối đa 150 ký tự")]
        public string Fullname { get; set; } = null!;

        [RegularExpression("^(Male|Female|Other)$", ErrorMessage = "Gender phải là Male, Female hoặc Other")]
        public string? Gender { get; set; }
        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string? Phone { get; set; }
        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string? Email { get; set; }

        [StringLength(255, ErrorMessage = "Địa chỉ  tối đa 255 ký tự")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "Password không được để trống")]
        [MinLength(6, ErrorMessage = "Password tối thiểu 6 ký tự")]
        public string PasswordHash { get; set; } = null!;

        // Staff properties
        [Required(ErrorMessage = "Lương  không được để trống")]
        [Range(0, double.MaxValue, ErrorMessage = "Salary phải >= 0")]
        public decimal? Salary { get; set; }
        [Required(ErrorMessage = "Vị trí làm việc không được để trống")]
        [StringLength(100, ErrorMessage = "Position tối đa 100 ký tự")]
        public string? Position { get; set; }

        [StringLength(50, ErrorMessage = "Số tài khoản tối đa 50 ký tự")]
        public string? BankAccountNumber { get; set; }

        [StringLength(100, ErrorMessage = "Tên ngân hàng tối đa 100 ký tự")]
        public string? BankName { get; set; }
        [Required(ErrorMessage = "Tax không được để trống")]
        [StringLength(20, ErrorMessage = "TaxId tối đa 20 ký tự")]
        public string? TaxId { get; set; }
    }
}

