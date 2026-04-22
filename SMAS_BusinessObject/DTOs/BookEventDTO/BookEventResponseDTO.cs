using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.DTOs.BookEventDTO
{
    public class BookEventResponseDTO
    {
        public int BookEventId { get; set; }
        public string? BookingCode { get; set; }
        public string? Status { get; set; }
        public int NumberOfTable { get; set; }
        public string TitleEvent { get; set; } = null!;
        public DateOnly ReservationDate { get; set; }
        public TimeOnly ReservationTime { get; set; }
    }
}
