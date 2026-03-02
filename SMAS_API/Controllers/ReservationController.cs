using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMAS_BusinessObject.DTOs.ReservationDTO;
using SMAS_Services.ReservationServices;

namespace SMAS_API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/reservation")]
    public class ReservationController : Controller
    {
        private readonly IReservationService _reservationservice;

        public ReservationController(IReservationService reservationservice)
        {
            _reservationservice = reservationservice;
        }

        [HttpGet]
        public async Task<ActionResult<ReservationListResponse>> GetAllReservation()
        {
            var  reservation = await _reservationservice.GetAllReservationsAsync();
            return Ok(reservation);
        }
    }
}
