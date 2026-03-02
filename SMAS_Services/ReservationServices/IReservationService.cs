using SMAS_BusinessObject.DTOs.ReservationDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Services.ReservationServices
{
    public interface IReservationService
    {
        Task<IEnumerable<ReservationListResponse>> GetAllReservationsAsync();

        
    }
}
