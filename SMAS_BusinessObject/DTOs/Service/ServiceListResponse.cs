using System;
using System.Collections.Generic;
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

}
