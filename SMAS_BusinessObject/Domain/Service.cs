using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.Domain
{
    public class Service
    {
        public int ServiceId { get; set; }          // FK

        public string Title { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public string? Description { get; set; }

        public int ReservationId { get; set; }      // PK 
    }
}
