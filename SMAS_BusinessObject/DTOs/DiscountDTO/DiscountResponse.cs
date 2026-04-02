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
        [Required(ErrorMessage = "Code is required")]
        [StringLength(50, ErrorMessage = "Code must not exceed 50 characters")]
        public string Code { get; set; } = null!;

        public string? Description { get; set; }

        [Required(ErrorMessage = "DiscountType is required")]
        public string DiscountType { get; set; } = null!; // e.g. "Percentage" | "FixedAmount"

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Value must be >= 0")]
        public decimal Value { get; set; }

        public decimal? MinOrderAmount { get; set; }
        public decimal? MaxDiscountAmount { get; set; }

        [Required(ErrorMessage = "StartDate is required")]
        public DateOnly StartDate { get; set; }

        [Required(ErrorMessage = "EndDate is required")]
        public DateOnly EndDate { get; set; }

        public int? UsageLimit { get; set; }
        public string? ApplicableFor { get; set; }
        public string? Status { get; set; }
        public int? CreatedBy { get; set; }
    }
    public class DiscountUpdateDto
    {
        public string? Description { get; set; }

        [Required(ErrorMessage = "DiscountType is required")]
        public string DiscountType { get; set; } = null!;

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Value must be >= 0")]
        public decimal Value { get; set; }

        public decimal? MinOrderAmount { get; set; }
        public decimal? MaxDiscountAmount { get; set; }

        [Required]
        public DateOnly StartDate { get; set; }

        [Required]
        public DateOnly EndDate { get; set; }

        public int? UsageLimit { get; set; }
        public string? ApplicableFor { get; set; }
        public string? Status { get; set; }
        public int? UsedCount { get; set; }
    }

    // DTO dùng để validate mã giảm giá
    public class DiscountValidateDto
    {
        [Required]
        public string Code { get; set; } = null!;
        public decimal OrderAmount { get; set; }
    }
}
