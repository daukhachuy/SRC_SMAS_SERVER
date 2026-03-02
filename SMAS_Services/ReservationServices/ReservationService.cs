using SMAS_BusinessObject.DTOs.ReservationDTO;
using SMAS_Repositories.ReservationRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Services.ReservationServices
{
    public class ReservationService : IReservationService
    {
        private readonly IReservationRepository _reservationRepository;

        public ReservationService(IReservationRepository reservationRepository)
        {
            _reservationRepository = reservationRepository;
        }

        public async Task<IEnumerable<ReservationListResponse>> GetAllReservationsAsync()
        {
            return await _reservationRepository.GetAllReservationsAsync();
        }
    }
}
