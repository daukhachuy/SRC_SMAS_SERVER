using System;
using System.Collections.Generic;
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

}
