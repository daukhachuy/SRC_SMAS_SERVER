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
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title must not exceed 200 characters")]
        public string Title { get; set; } = null!;

        public string? Description { get; set; }
        public string? EventType { get; set; }
        public string? Image { get; set; }

        public int? MinGuests { get; set; }
        public int? MaxGuests { get; set; }

        public decimal? BasePrice { get; set; }

        [Required(ErrorMessage = "CreatedBy is required")]
        public int CreatedBy { get; set; }

        public bool? IsActive { get; set; }
    }

    public class EventUpdateDto
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title must not exceed 200 characters")]
        public string Title { get; set; } = null!;

        public string? Description { get; set; }
        public string? EventType { get; set; }
        public string? Image { get; set; }
        public int? MinGuests { get; set; }
        public int? MaxGuests { get; set; }
        public decimal? BasePrice { get; set; }
        public bool? IsActive { get; set; }
    }

}
