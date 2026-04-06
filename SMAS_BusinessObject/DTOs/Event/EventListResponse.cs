using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.DTOs.Event
{
    public class EventListResponse
    {
        public int EventId { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public string? EventType { get; set; }
        public string? Image { get; set; }
        public int? MinGuests { get; set; }
        public int? MaxGuests { get; set; }
        public decimal? BasePrice { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int CreatedBy { get; set; }
        public bool? IsActive { get; set; }
    }
    public class EventCreateDto
    {
        [Required(ErrorMessage = "Tiêu đề không được để trống.")]
        [MaxLength(500)]
        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        [MaxLength(100)]
        public string? EventType { get; set; }

        public string? Image { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Số khách tối thiểu phải >= 1.")]
        public int? MinGuests { get; set; }

        [MaxGuestsGreaterThanMin]
        [Range(1, int.MaxValue, ErrorMessage = "Số khách tối đa phải >= 1.")]
        public int? MaxGuests { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Giá cơ bản phải lớn hơn 0.")]
        public decimal? BasePrice { get; set; }

        public bool? IsActive { get; set; } = true;

        [Required(ErrorMessage = "CreatedBy không được để trống.")]
        public int CreatedBy { get; set; }
    }

    public class EventUpdateDto
    {
        [Required(ErrorMessage = "Tiêu đề không được để trống.")]
        [MaxLength(500)]
        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        [MaxLength(100)]
        public string? EventType { get; set; }

        public string? Image { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Số khách tối thiểu phải >= 1.")]
        public int? MinGuests { get; set; }

        [MaxGuestsGreaterThanMin]
        [Range(1, int.MaxValue, ErrorMessage = "Số khách tối đa phải >= 1.")]
        public int? MaxGuests { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Giá cơ bản phải lớn hơn 0.")]
        public decimal? BasePrice { get; set; }

        public bool? IsActive { get; set; }
    }

    // Custom validation: MaxGuests phải >= MinGuests
    public class MaxGuestsGreaterThanMinAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext context)
        {
            var minProp = context.ObjectType.GetProperty("MinGuests");
            if (minProp == null) return ValidationResult.Success;

            var minGuests = minProp.GetValue(context.ObjectInstance) as int?;
            var maxGuests = value as int?;

            if (minGuests.HasValue && maxGuests.HasValue && maxGuests.Value < minGuests.Value)
                return new ValidationResult(
                    $"Số khách tối đa ({maxGuests.Value}) phải >= số khách tối thiểu ({minGuests.Value}).");

            return ValidationResult.Success;
        }
    }

    }
