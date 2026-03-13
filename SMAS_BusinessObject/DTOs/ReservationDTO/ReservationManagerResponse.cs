using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.DTOs.ReservationDTO
{
    public class ReservationManagerResponse
    {
        public int sumReservationsToday { get; set; }
        public int sumReservationAwaitConfirm { get; set; }
    }
}
