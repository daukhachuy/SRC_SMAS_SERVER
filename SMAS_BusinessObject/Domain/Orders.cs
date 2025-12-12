using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.Domain
{
    public class Orders
    {
        public int OrderId { get; set; }              // PK

        public int UserId { get; set; }               // FK

        public string? Note { get; set; }

        public DateTime CreatedAt { get; set; }

        public int NumberOfGuests { get; set; }

        public string OrderStatus { get; set; } = string.Empty;

        public string OrderType { get; set; } = string.Empty;

        public int? ReservationId { get; set; }       // FK (có thể null)

        public int? ComboId { get; set; }             // FK (có thể null)
    }
}
