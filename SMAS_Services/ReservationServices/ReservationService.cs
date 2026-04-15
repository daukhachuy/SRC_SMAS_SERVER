using SMAS_BusinessObject.DTOs.ReservationDTO;
using SMAS_BusinessObject.Models;
using SMAS_DataAccess.DAO;
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

        public async Task<ReservationListResponse> CreatePendingReservation(ReservationCreateRequestDTO dto, int userid)
        {
            //bool isDuplicate = await _reservationRepository.CheckDuplicateReservation(userid, dto.ReservationDate, dto.ReservationTime);

            //if (isDuplicate)
            //{
            //    return null; 
            //}
            string uniqueCode = GenerateCode();
            while (_reservationRepository.CheckCodeExists(uniqueCode))
            {
                uniqueCode = GenerateCode();
            }


            var newReservation = new Reservation
            {
                UserId = userid,
                ReservationCode = uniqueCode,
                ReservationDate = dto.ReservationDate,
                ReservationTime = dto.ReservationTime,
                NumberOfGuests = dto.NumberOfGuests,
                SpecialRequests = dto.SpecialRequests,
                Status = "Pending", 
                CreatedAt = DateTime.Now
            };


            var response = await _reservationRepository.CreatePendingReservation(newReservation);
            return response;
        }
        private string GenerateCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 8)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public async Task<IEnumerable<ReservationListResponse>> GetMyReservationsAsync(int userId)
    => await _reservationRepository.GetReservationsByUserIdAsync(userId);
    }
}
