using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.DTOs.BuffetDTO
{
    public class BuffetListResponseDTO
    {
        public int BuffetId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal MainPrice { get; set; }
        public decimal? ChildrenPrice { get; set; }
        public decimal? SidePrice { get; set; }
        public string? Image { get; set; }
        public bool? IsAvailable { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

    }
    public class BuffetCreateDto
    {
        [Required(ErrorMessage = "Tên buffet không được để trống.")]
        [MaxLength(200)]
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        [Required(ErrorMessage = "Giá chính không được để trống.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Giá chính phải lớn hơn 0.")]
        public decimal MainPrice { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Giá trẻ em phải lớn hơn 0.")]
        [PriceLessThanMainPrice(nameof(MainPrice), "Giá trẻ em")]
        public decimal? ChildrenPrice { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Giá phụ phải lớn hơn 0.")]
        [PriceLessThanMainPrice(nameof(MainPrice), "Giá phụ")]
        public decimal? SidePrice { get; set; }

        public string? Image { get; set; }
        public bool? IsAvailable { get; set; } = true;
        public int? CreatedBy { get; set; }
    }

    public class BuffetUpdateDto
    {
        [Required(ErrorMessage = "Tên buffet không được để trống.")]
        [MaxLength(200)]
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        [Required(ErrorMessage = "Giá chính không được để trống.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Giá chính phải lớn hơn 0.")]
        public decimal MainPrice { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Giá trẻ em phải lớn hơn 0.")]
        [PriceLessThanMainPrice(nameof(MainPrice), "Giá trẻ em")]
        public decimal? ChildrenPrice { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Giá phụ phải lớn hơn 0.")]
        [PriceLessThanMainPrice(nameof(MainPrice), "Giá phụ")]
        public decimal? SidePrice { get; set; }

        public string? Image { get; set; }
        public bool? IsAvailable { get; set; }
    }

    // Custom validation: ChildrenPrice / SidePrice phải nhỏ hơn MainPrice
    public class PriceLessThanMainPriceAttribute : ValidationAttribute
    {
        private readonly string _mainPricePropertyName;
        private readonly string _fieldLabel;

        public PriceLessThanMainPriceAttribute(string mainPricePropertyName, string fieldLabel)
        {
            _mainPricePropertyName = mainPricePropertyName;
            _fieldLabel = fieldLabel;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext context)
        {
            var mainPriceProp = context.ObjectType.GetProperty(_mainPricePropertyName);
            if (mainPriceProp == null) return ValidationResult.Success;

            var mainPrice = (decimal)mainPriceProp.GetValue(context.ObjectInstance)!;
            var price = value as decimal?;

            if (price.HasValue && price.Value >= mainPrice)
                return new ValidationResult(
                    $"{_fieldLabel} ({price.Value}) phải nhỏ hơn giá chính ({mainPrice}).");

            return ValidationResult.Success;
        }
    }
    }
