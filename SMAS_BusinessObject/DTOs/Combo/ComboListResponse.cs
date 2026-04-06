using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.DTOs.Combo
{
    public class ComboListResponse
    {
        public int ComboId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountPercent { get; set; }
        public string? Image { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? ExpiryDate { get; set; }
        public int? NumberOfUsed { get; set; }
        public int? MaxUsage { get; set; }
        public bool? IsAvailable { get; set; }
        public int? CreatedBy { get; set; }

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public List<ComboFoodItemDto> Foods { get; set; } = new List<ComboFoodItemDto>();
    }
    public class ComboCreateDto
    {
        [Required(ErrorMessage = "Tên combo không được để trống.")]
        [MaxLength(200)]
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        [Required(ErrorMessage = "Giá không được để trống.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Giá phải lớn hơn 0.")]
        public decimal Price { get; set; }

        [Range(0, 100, ErrorMessage = "Phần trăm giảm giá phải từ 0 đến 100.")]
        public decimal? DiscountPercent { get; set; }

        public string? Image { get; set; }

        public DateOnly? StartDate { get; set; }

        [ExpiryDateAfterStartDate]
        public DateOnly? ExpiryDate { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "MaxUsage phải >= 0.")]
        public int? MaxUsage { get; set; }

        public bool? IsAvailable { get; set; } = true;

        public int? CreatedBy { get; set; }
    }

    public class ComboUpdateDto
    {
        [Required(ErrorMessage = "Tên combo không được để trống.")]
        [MaxLength(200)]
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        [Required(ErrorMessage = "Giá không được để trống.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Giá phải lớn hơn 0.")]
        public decimal Price { get; set; }

        [Range(0, 100, ErrorMessage = "Phần trăm giảm giá phải từ 0 đến 100.")]
        public decimal? DiscountPercent { get; set; }

        public string? Image { get; set; }

        public DateOnly? StartDate { get; set; }

        [ExpiryDateAfterStartDate]
        public DateOnly? ExpiryDate { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "MaxUsage phải >= 0.")]
        public int? MaxUsage { get; set; }

        public bool? IsAvailable { get; set; }
    }
    public class ExpiryDateAfterStartDateAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext context)
        {
            var startDateProp = context.ObjectType.GetProperty("StartDate");
            if (startDateProp == null) return ValidationResult.Success;

            var startDate = startDateProp.GetValue(context.ObjectInstance) as DateOnly?;
            var expiryDate = value as DateOnly?;

            if (startDate.HasValue && expiryDate.HasValue && expiryDate.Value <= startDate.Value)
                return new ValidationResult(
                    $"Ngày hết hạn ({expiryDate.Value}) phải sau ngày bắt đầu ({startDate.Value}).");

            return ValidationResult.Success;
        }
    }
    public class ComboFoodItemDto
    {
        public int FoodId { get; set; }
        public string FoodName { get; set; } = null!;
        public string? FoodImage { get; set; }
        public int Quantity { get; set; }
        public decimal? FoodPrice { get; set; }  
    }
}
