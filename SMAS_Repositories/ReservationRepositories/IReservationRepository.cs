using SMAS_BusinessObject.DTOs.ReservationDTO;
using SMAS_BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Repositories.ReservationRepositories
{
    public interface IReservationRepository
    {
        Task<IEnumerable<ReservationListResponse>> GetAllReservationsAsync();

        Task<IEnumerable<ReservationListResponse>> GetAllReservationsByStatusAsync(String status);

        Task<ReservationListResponse> CreatePendingReservation(Reservation request);

        Task<bool> CheckDuplicateReservation(int userId, DateOnly date, TimeOnly time);

        bool CheckCodeExists(string code);
        Task<IEnumerable<ReservationListResponse>> GetReservationsByUserIdAsync(int userId);

        Task<IEnumerable<SearchReservationResponseDTO>> GetAllReservationsByStatusAsync(string status, string request);
    }
}
