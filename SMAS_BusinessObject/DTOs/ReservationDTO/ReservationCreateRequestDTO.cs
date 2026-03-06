using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.DTOs.ReservationDTO
{
    public  class ReservationCreateRequestDTO
    {
        [Required(ErrorMessage = "Vui lòng chọn ngày đặt chỗ.")]
        [ValidReservationDate]
        public DateOnly ReservationDate { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn giờ đặt chỗ.")]
        [ValidReservationTime]
        public TimeOnly ReservationTime { get; set; }
        [Required(ErrorMessage = "Vui lòng khồng để trống Số lượng khách .")]
        [Range(1, 29, ErrorMessage = "Số lượng khách phải lớn hơn 0 nếu lớn hơn 30 vui lòng đặt sự kiện .")]
        public int NumberOfGuests { get; set; }

        [MaxLength(1000, ErrorMessage = "Yêu cầu đặc biệt không quá 1000 ký tự.")]
        public string? SpecialRequests { get; set; }
    }

    public class ValidReservationDateAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is DateOnly date)
            {
                var today = DateOnly.FromDateTime(DateTime.Now);
                var maxDate = today.AddDays(5);

                if (date < today)
                    return new ValidationResult("Ngày đặt chỗ không được ở trong quá khứ.");

                if (date > maxDate)
                    return new ValidationResult("Bạn chỉ có thể đặt chỗ trước tối đa 5 ngày.");

                return ValidationResult.Success;
            }
            return new ValidationResult("Ngày không hợp lệ.");
        }
    }

    // Kiểm tra giờ đặt chỗ: Từ 8h sáng đến 9h tối (21h)
    public class ValidReservationTimeAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is TimeOnly time)
            {
                var startTime = new TimeOnly(8, 0);
                var endTime = new TimeOnly(21, 0);

                if (time < startTime || time > endTime)
                    return new ValidationResult("Giờ đặt chỗ phải nằm trong khoảng từ 08:00 đến 21:00.");

                return ValidationResult.Success;
            }
            return new ValidationResult("Giờ không hợp lệ.");
        }
    }
}
