using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.Domain
{
    public class Event
    {
        [Key]
        public int EventId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        public string? Content { get; set; }

        public string? Description { get; set; }

        // Navigation
        public virtual ICollection<Reservation>? Reservations { get; set; }
    }
}
