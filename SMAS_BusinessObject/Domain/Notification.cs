using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.Domain
{
    public class Notification
    {
        public int NotificationId { get; set; }           // PK

        public string Title { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;

        public string? Severity { get; set; }

        public bool IsRead { get; set; }

        public DateTime CreatedAt { get; set; }

        public int? ReservationId { get; set; }           // FK có thể null

        public int? ProductId { get; set; }               // FK có thể null

        public int? IngredientId { get; set; }            // FK có thể null

        public int UserId { get; set; }                  // FK
    }
}