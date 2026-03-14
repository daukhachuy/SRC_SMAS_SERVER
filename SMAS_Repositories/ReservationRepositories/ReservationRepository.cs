using SMAS_BusinessObject.DTOs.ReservationDTO;
using SMAS_BusinessObject.Models;
using SMAS_DataAccess.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Repositories.ReservationRepositories
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly ReservationDAO _reservation;

        public ReservationRepository(ReservationDAO reservation)
        {
            _reservation = reservation;
        }

        public async Task<IEnumerable<ReservationListResponse>> GetAllReservationsAsync()
        {
           var reservations = await _reservation.GetAllReservationsAsync();
            return reservations.Select(r => new ReservationListResponse
            {
                UserId = r.UserId,
                Fullname = r.User.Fullname,
                Phone = r.User.Phone,
                Email = r.User.Email,
                ReservationId = r.ReservationId,
                ReservationCode = r.ReservationCode,
                ReservationDate = r.ReservationDate,
                ReservationTime = r.ReservationTime,
                NumberOfGuests = r.NumberOfGuests,
                SpecialRequests = r.SpecialRequests,
                Status = r.Status,
                ConfirmedAt = r.ConfirmedAt,
                CancelledAt = r.CancelledAt,
                CancellationReason = r.CancellationReason,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt,
                ConfirmedBy = r.ConfirmedBy,
                ConfirmedByName = r.ConfirmedByNavigation != null ? r.ConfirmedByNavigation.User.Fullname : null
            }).ToList();
        }

        public async Task<IEnumerable<ReservationListResponse>> GetAllReservationsByStatusAsync(String status)
        {
            var reservations = await GetAllReservationsAsync();
            return  reservations.Where(r => r.Status.Equals(status, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public async Task<ReservationListResponse> CreatePendingReservation(Reservation request)
        {
            var r = await _reservation.AddReservation(request);

            return new ReservationListResponse
            {
                UserId = r.UserId,
                Fullname = r.User.Fullname,
                Phone = r.User.Phone,
                Email = r.User.Email,
                ReservationId = r.ReservationId,
                ReservationCode = r.ReservationCode,
                ReservationDate = r.ReservationDate,
                ReservationTime = r.ReservationTime,
                NumberOfGuests = r.NumberOfGuests,
                SpecialRequests = r.SpecialRequests,
                Status = r.Status,
                ConfirmedAt = r.ConfirmedAt,
                CancelledAt = r.CancelledAt,
                CancellationReason = r.CancellationReason,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt,
                ConfirmedBy = r.ConfirmedBy,
                ConfirmedByName = r.ConfirmedByNavigation != null ? r.ConfirmedByNavigation.User.Fullname : null
            };
        }
        public async Task<IEnumerable<ReservationListResponse>> GetReservationsByUserIdAsync(int userId)
        {
            var reservations = await _reservation.GetReservationsByUserIdAsync(userId);
            return reservations.Select(r => MapToDto(r)).ToList();
        }
        private static ReservationListResponse MapToDto(Reservation r) =>
            new()
            {
                UserId = r.UserId,
                Fullname = r.User.Fullname,
                Phone = r.User.Phone,
                Email = r.User.Email,
                ReservationId = r.ReservationId,
                ReservationCode = r.ReservationCode,
                ReservationDate = r.ReservationDate,
                ReservationTime = r.ReservationTime,
                NumberOfGuests = r.NumberOfGuests,
                SpecialRequests = r.SpecialRequests,
                Status = r.Status,
                ConfirmedAt = r.ConfirmedAt,
                CancelledAt = r.CancelledAt,
                CancellationReason = r.CancellationReason,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt,
                ConfirmedBy = r.ConfirmedBy,
                ConfirmedByName = r.ConfirmedByNavigation != null ? r.ConfirmedByNavigation.User.Fullname : null
            };


        public async Task<bool> CheckDuplicateReservation(int userId, DateOnly date, TimeOnly time) => await _reservation.CheckDuplicateReservation(userId, date, time);
        public bool CheckCodeExists(string code) => _reservation.CheckCodeExists(code);

    }
}
