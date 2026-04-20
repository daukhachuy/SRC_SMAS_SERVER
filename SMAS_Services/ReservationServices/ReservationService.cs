using SMAS_BusinessObject.DTOs.ReservationDTO;
using SMAS_BusinessObject.Models;
using SMAS_DataAccess.DAO;
using SMAS_Repositories.ReservationRepositories;
using SMAS_Services.NotificationServices;
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
        private readonly INotificationService _notificationService;
        public ReservationService(IReservationRepository reservationRepository, INotificationService notificationService)
        {
            _reservationRepository = reservationRepository;
            _notificationService = notificationService;
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
            if (response != null && response.ReservationId > 0)
            {
                try
                {
                    var managers = await _reservationRepository.GetUsersByRoleAsync("Manager");
                    Console.WriteLine($"[DEBUG] Found {managers.Count} managers");

                    foreach (var manager in managers)
                    {
                        Console.WriteLine($"[DEBUG] Sending notification to manager userId={manager.UserId}");
                        await _notificationService.CreateAutoNotificationAsync(
                            userId: manager.UserId,
                            senderId: userid,
                            title: "Có đặt bàn mới cần xác nhận",
                            content: $"Khách {response.Fullname} đặt bàn {uniqueCode} " +
                                     $"ngày {dto.ReservationDate:dd/MM/yyyy} lúc {dto.ReservationTime:HH\\:mm} " +
                                     $"cho {dto.NumberOfGuests} khách.",
                            type: "Reservation",
                            severity: "Information"
                        );
                        Console.WriteLine($"[DEBUG] Notification sent to manager userId={manager.UserId}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] Notification failed: {ex.Message}");
                    Console.WriteLine($"[ERROR] StackTrace: {ex.StackTrace}");
                }
            }
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

        public async Task<IEnumerable<SearchReservationResponseDTO>> CheckAvailabilityAsync(string request)
        {
            return await _reservationRepository.GetAllReservationsByStatusAsync("Confirmed" , request);
        }
    }
}
