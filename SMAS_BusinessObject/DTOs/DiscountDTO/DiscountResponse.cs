using SMAS_BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.DTOs.DiscountDTO
{
    public class DiscountResponse
    {
        public int DiscountId { get; set; }

        public string Code { get; set; } = null!;

        public string? Description { get; set; }

        public string DiscountType { get; set; } = null!;

        public decimal Value { get; set; }

        public decimal? MinOrderAmount { get; set; }

        public decimal? MaxDiscountAmount { get; set; }

        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }

        public int? UsageLimit { get; set; }

        public int? UsedCount { get; set; }

        public string? ApplicableFor { get; set; }

        public string? Status { get; set; }

        public DateTime? CreatedAt { get; set; }

        public  int? CreatedBy { get; set; } = null!;

    }

    public class DiscountCreateDto
    {
        [Required(ErrorMessage = "Mã giảm giá không được để trống.")]
        [MaxLength(100)]
        public string Code { get; set; } = null!;

        public string? Description { get; set; }

        [Required(ErrorMessage = "Loại giảm giá không được để trống.")]
        [MaxLength(50)]
        [AllowedValues("Percentage", "Fixed", ErrorMessage = "DiscountType phải là 'Percentage' hoặc 'Fixed'.")]
        public string DiscountType { get; set; } = null!;

        [Required(ErrorMessage = "Giá trị không được để trống.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Giá trị phải lớn hơn 0.")]
        public decimal Value { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Giá đơn hàng tối thiểu phải lớn hơn 0.")]
        public decimal? MinOrderAmount { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Giảm giá tối đa phải lớn hơn 0.")]
        public decimal? MaxDiscountAmount { get; set; }

        [Required(ErrorMessage = "Ngày bắt đầu không được để trống.")]
        public DateOnly StartDate { get; set; }

        [Required(ErrorMessage = "Ngày kết thúc không được để trống.")]
        [EndDateAfterStartDate]
        public DateOnly EndDate { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Giới hạn sử dụng phải >= 1.")]
        public int? UsageLimit { get; set; }

        [MaxLength(100)]
        [AllowedValues("All", "DineIn", "Takeaway", "Delivery", ErrorMessage = "ApplicableFor phải là 'All', 'DineIn', 'Takeaway' hoặc 'Delivery'.")]
        public string? ApplicableFor { get; set; }

        [MaxLength(50)]
        public string? Status { get; set; } = "Active";

        public int? CreatedBy { get; set; }
    }

    public class DiscountUpdateDto
    {
        [Required(ErrorMessage = "Mã giảm giá không được để trống.")]
        [MaxLength(100)]
        public string Code { get; set; } = null!;

        public string? Description { get; set; }

        [Required(ErrorMessage = "Loại giảm giá không được để trống.")]
        [MaxLength(50)]
        [AllowedValues("Percentage", "Fixed", ErrorMessage = "DiscountType phải là 'Percentage' hoặc 'Fixed'.")]
        public string DiscountType { get; set; } = null!;

        [Required(ErrorMessage = "Giá trị không được để trống.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Giá trị phải lớn hơn 0.")]
        public decimal Value { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Giá đơn hàng tối thiểu phải lớn hơn 0.")]
        public decimal? MinOrderAmount { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Giảm giá tối đa phải lớn hơn 0.")]
        public decimal? MaxDiscountAmount { get; set; }

        [Required(ErrorMessage = "Ngày bắt đầu không được để trống.")]
        public DateOnly StartDate { get; set; }

        [Required(ErrorMessage = "Ngày kết thúc không được để trống.")]
        [EndDateAfterStartDate]
        public DateOnly EndDate { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Giới hạn sử dụng phải >= 1.")]
        public int? UsageLimit { get; set; }

        [MaxLength(100)]
        [AllowedValues("All", "DineIn", "Takeaway", "Delivery", ErrorMessage = "ApplicableFor phải là 'All', 'DineIn', 'Takeaway' hoặc 'Delivery'.")]
        public string? ApplicableFor { get; set; }

        [MaxLength(50)]
        public string? Status { get; set; }
    }

    // Custom validation: EndDate phải sau StartDate
    public class EndDateAfterStartDateAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext context)
        {
            var startProp = context.ObjectType.GetProperty("StartDate");
            if (startProp == null) return ValidationResult.Success;

            var startDate = (DateOnly)startProp.GetValue(context.ObjectInstance)!;
            var endDate = (DateOnly)value!;

            if (endDate <= startDate)
                return new ValidationResult(
                    $"Ngày kết thúc ({endDate}) phải sau ngày bắt đầu ({startDate}).");

            return ValidationResult.Success;
        }
    }
    }
