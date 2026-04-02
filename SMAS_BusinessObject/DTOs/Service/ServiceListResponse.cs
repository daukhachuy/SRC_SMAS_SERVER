using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.DTOs.Service
{
    public class ServiceListResponse
    {
        public int ServiceId { get; set; }
        public string Title { get; set; } = null!;
        public decimal ServicePrice { get; set; }
        public string? Description { get; set; }
        public string? Unit { get; set; }
        public string? Image { get; set; }
        public bool? IsAvailable { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class ServiceCreateDto
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title must not exceed 200 characters")]
        public string Title { get; set; } = null!;

        [Required(ErrorMessage = "ServicePrice is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be >= 0")]
        public decimal ServicePrice { get; set; }

        public string? Description { get; set; }
        public string? Unit { get; set; }
        public string? Image { get; set; }
        public bool? IsAvailable { get; set; }
    }

    public class ServiceUpdateDto
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title must not exceed 200 characters")]
        public string Title { get; set; } = null!;

        [Required(ErrorMessage = "ServicePrice is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be >= 0")]
        public decimal ServicePrice { get; set; }

        public string? Description { get; set; }
        public string? Unit { get; set; }
        public string? Image { get; set; }
        public bool? IsAvailable { get; set; }
    }

}
