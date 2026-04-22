using SMAS_BusinessObject.DTOs.ManagerDTO;
using SMAS_BusinessObject.DTOs.OrderDTO;
using SMAS_BusinessObject.DTOs.ReservationDTO;
using SMAS_BusinessObject.Models;
using SMAS_DataAccess.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMAS_Repositories.ManagerRepositories
{
    public class ManagerRepository : IManagerRepository
    {
        private readonly ManagerDAO _managerDAO;

        private static readonly List<(string Name, TimeOnly Start, TimeOnly End)> TimeSlots = new()
        {
            ("Sáng",  new TimeOnly(9, 0),  new TimeOnly(12, 0)),
            ("Trưa",  new TimeOnly(12, 0), new TimeOnly(15, 0)),
            ("Chiều", new TimeOnly(15, 0), new TimeOnly(19, 0)),
            ("Tối",   new TimeOnly(19, 0), new TimeOnly(22, 0))
        };

        public ManagerRepository(ManagerDAO managerDAO)
        {
            _managerDAO = managerDAO;
        }

        public async Task<TableAvailabilityResponseDTO> GetTableAvailabilityAsync(DateOnly date, string? timeSlot)
        {
            var reservations = await _managerDAO.GetConfirmedReservationsByDateAsync(date);
            var bookEvents = await _managerDAO.GetConfirmedBookEventsByDateAsync(date);
            int totalTables = await _managerDAO.CountActiveTablesAsync();

            var now = DateTime.Now;
            var today = DateOnly.FromDateTime(now);
            var currentTime = TimeOnly.FromDateTime(now);

            var slotsToProcess = string.IsNullOrWhiteSpace(timeSlot)
                ? TimeSlots
                : TimeSlots.Where(s => s.Name.Equals(timeSlot, StringComparison.OrdinalIgnoreCase)).ToList();

            var result = new TableAvailabilityResponseDTO
            {
                Date = date,
                TimeSlots = new List<TimeSlotAvailabilityDTO>()
            };

            foreach (var (slotName, slotStart, slotEnd) in slotsToProcess)
            {
                bool isCurrentSlot = date == today && currentTime >= slotStart && currentTime < slotEnd;

                var slotReservations = reservations
                    .Where(r => r.ReservationTime >= slotStart && r.ReservationTime < slotEnd)
                    .ToList();

                var slotBookEvents = bookEvents
                    .Where(be => be.ReservationTime >= slotStart && be.ReservationTime < slotEnd)
                    .ToList();

                var reservationDtos = slotReservations.Select(r =>
                {
                    string? orderCode = null;
                    if (isCurrentSlot)
                    {
                        orderCode = r.Orders
                            .Where(o => o.OrderStatus != "Cancelled" && o.OrderStatus != "Closed" && o.OrderStatus != "Completed")
                            .Select(o => o.OrderCode)
                            .FirstOrDefault();
                    }

                    return new ReservationSlotDTO
                    {
                        ReservationCode = r.ReservationCode,
                        NumberOfGuests = r.NumberOfGuests,
                        ReservationTime = r.ReservationTime,
                        CustomerName = r.User?.Fullname,
                        CustomerPhone = r.User?.Phone,
                        OrderCode = orderCode
                    };
                }).ToList();

                var bookEventDtos = slotBookEvents.Select(be =>
                {
                    string? orderCode = null;
                    if (isCurrentSlot)
                    {
                        orderCode = be.Orders
                            .Where(o => o.OrderStatus != "Cancelled" && o.OrderStatus != "Closed" && o.OrderStatus != "Completed")
                            .Select(o => o.OrderCode)
                            .FirstOrDefault();
                    }

                    return new BookEventSlotDTO
                    {
                        BookingCode = be.BookingCode,
                        NumberOfGuests = be.NumberOfGuests,
                        ReservationTime = be.ReservationTime,
                        CustomerName = be.Customer?.Fullname,
                        EventTitle = be.Event?.Title,
                        OrderCode = orderCode
                    };
                }).ToList();

                int totalGuests = slotReservations.Sum(r => r.NumberOfGuests);
                int reservationTables = slotReservations
                    .Sum(r => (int)Math.Ceiling(r.NumberOfGuests / 4.0));
                int bookEventTables = slotBookEvents.Sum(be => be.NumberOfGuests);
                int bookedTables = reservationTables + bookEventTables;
                double capacity = totalTables > 0
                    ? Math.Round((double)bookedTables / totalTables * 100, 1)
                    : 0;

                result.TimeSlots.Add(new TimeSlotAvailabilityDTO
                {
                    TimeSlotName = slotName,
                    StartTime = slotStart,
                    EndTime = slotEnd,
                    Reservations = reservationDtos,
                    BookEvents = bookEventDtos,
                    Summary = new SlotSummaryDTO
                    {
                        TotalGuests = totalGuests,
                        TotalBookedTables = Math.Min(bookedTables, totalTables),
                        ActiveTables = totalTables,
                        CapacityPercentage = Math.Min(capacity, 100)
                    }
                });
            }

            return result;
        }

        public async Task<IEnumerable<OrderTodayResponseDTO>> GetOrdersTodayAsync()
        {
            var orders = await _managerDAO.GetOrdersTodayAsync();
            return orders.Select(MapOrderToDto).ToList();
        }

        public async Task<IEnumerable<TableEmptyResponseDTO>> GetEmptyTablesAsync()
        {
            var tables = await _managerDAO.GetEmptyTablesAsync();
            return tables.Select(t => new TableEmptyResponseDTO
            {
                TableId = t.TableId,
                TableName = t.TableName,
                TableType = t.TableType,
                NumberOfPeople = t.NumberOfPeople,
                Status = t.Status,
                QrCode = t.QrCode,
                IsActive = t.IsActive
            }).ToList();
        }

        // ManagerRepository.cs - bỏ deconstruct tuple, gọi thẳng
        public async Task<RevenueWeekResponseDTO> GetRevenuePreviousSevenDaysAsync()
        {
            return await _managerDAO.GetRevenuePreviousSevenDaysAsync();
        }

        public async Task<IEnumerable<OrderTodayResponseDTO>> GetFourNewestOrdersAsync()
        {
            var orders = await _managerDAO.GetFourNewestOrdersAsync();
            return orders.Select(MapOrderToDto).ToList();
        }

        public async Task<IEnumerable<StaffWorkTodayResponseDTO>> GetStaffWorkTodayAsync()
        {
            var workStaffs = await _managerDAO.GetStaffWorkTodayAsync();
            return workStaffs.Select(ws => new StaffWorkTodayResponseDTO
            {
                UserId = ws.UserId,
                Fullname = ws.User?.Fullname ?? "",
                Phone = ws.User?.Phone,
                Email = ws.User?.Email,
                Role = ws.User?.Role,
                Position = ws.User?.Staff?.Position,
                ShiftId = ws.ShiftId,
                ShiftName = ws.Shift?.ShiftName,
                CheckInTime = ws.CheckInTime,
                CheckOutTime = ws.CheckOutTime,
                IsWorking = ws.IsWorking
            }).ToList();
        }

        public async Task<IEnumerable<NotificationResponseDTO>> GetNotificationsByUserIdAsync(int userId)
        {
            var notifications = await _managerDAO.GetNotificationsByUserIdAsync(userId);
            return notifications.Select(n => new NotificationResponseDTO
            {
                NotificationId = n.NotificationId,
                UserId = n.UserId,
                SenderId = n.SenderId,
                Title = n.Title,
                Content = n.Content,
                Type = n.Type,
                Severity = n.Severity,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt,
                ReadAt = n.ReadAt
            }).ToList();
        }

        public async Task<SumReservationTodayResponseDTO> GetSumReservationTodayAsync()
        {
            var count = await _managerDAO.GetSumReservationTodayAsync();
            return new SumReservationTodayResponseDTO { TotalCount = count };
        }

        public async Task<IEnumerable<ReservationListResponse>> GetReservationsWaitConfirmAsync()
        {
            var reservations = await _managerDAO.GetReservationsWaitConfirmAsync();
            return reservations.Select(MapReservationToDto).ToList();
        }

        public async Task<IEnumerable<ReservationListResponse>> GetAllReservationsDescCreatedAtAsync()
        {
            var reservations = await _managerDAO.GetAllReservationsDescCreatedAtAsync();
            return reservations.Select(MapReservationToDto).ToList();
        }

        public async Task<IEnumerable<BookEventListResponseDTO>> GetAllBookEventsAscCreatedAtAsync()
        {
            var bookEvents = await _managerDAO.GetBookEventsAscCreatedAtAsync();
            return bookEvents.Select(MapBookEventToDto).ToList();
        }

        public async Task<IEnumerable<UpcomingEventResponseDTO>> GetUpcomingEventsAsync()
        {
            var bookEvents = await _managerDAO.GetUpcomingBookEventsAsync();
            return bookEvents.Select(be => new UpcomingEventResponseDTO
            {
                BookEventId = be.BookEventId,
                BookingCode = be.BookingCode,
                EventId = be.EventId,
                EventTitle = be.Event?.Title,
                EventType = be.Event?.EventType,
                CustomerId = be.CustomerId,
                CustomerName = be.Customer?.Fullname,
                CustomerPhone = be.Customer?.Phone,
                NumberOfGuests = be.NumberOfGuests,
                ReservationDate = be.ReservationDate,
                ReservationTime = be.ReservationTime,
                Status = be.Status,
                TotalAmount = be.TotalAmount,
                CreatedAt = be.CreatedAt
            }).ToList();
        }

        public async Task<NumberContractNeedSignedResponseDTO> GetNumberContractNeedSignedAsync()
        {
            var count = await _managerDAO.GetNumberContractNeedSignedAsync();
            return new NumberContractNeedSignedResponseDTO { TotalCount = count };
        }

        public async Task<bool> DeleteReservationByReservationCodeAsync(string reservationCode, string cancellationReason, int? managerUserId)
        {
            return await _managerDAO.DeleteReservationByCodeAsync(reservationCode, cancellationReason, managerUserId);
        }

        public async Task<ReservationListResponse?> PatchConfirmReservationAsync(string reservationCode, int? managerUserId)
        {
            var reservation = await _managerDAO.UpdateReservationConfirmAsync(reservationCode, managerUserId);
            return reservation == null ? null : MapReservationToDto(reservation);
        }

        private static ReservationListResponse MapReservationToDto(Reservation r)
        {
            return new ReservationListResponse
            {
                UserId = r.UserId,
                Fullname = r.User?.Fullname ?? "",
                Phone = r.User?.Phone,
                Email = r.User?.Email,
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
                ConfirmedByName = r.ConfirmedByNavigation?.User?.Fullname
            };
        }

        private static BookEventListResponseDTO MapBookEventToDto(BookEvent be)
        {
            return new BookEventListResponseDTO
            {
                BookEventId = be.BookEventId,
                BookingCode = be.BookingCode,
                EventId = be.EventId,
                EventTitle = be.Event?.Title,
                CustomerId = be.CustomerId,
                CustomerName = be.Customer?.Fullname,
                CustomerPhone = be.Customer?.Phone,
                CustomerEmail = be.Customer?.Email,
                NumberOfGuests = be.NumberOfGuests,
                ReservationDate = be.ReservationDate,
                ReservationTime = be.ReservationTime,
                Status = be.Status,
                TotalAmount = be.TotalAmount,
                CreatedAt = be.CreatedAt,
                UpdatedAt = be.UpdatedAt,
                ConfirmedAt = be.ConfirmedAt,
                ConfirmedBy = be.ConfirmedBy,
                ConfirmedByName = be.ConfirmedByNavigation?.User?.Fullname
            };
        }

        private static OrderTodayResponseDTO MapOrderToDto(Order o)
        {
            return new OrderTodayResponseDTO
            {
                OrderId = o.OrderId,
                OrderCode = o.OrderCode,
                OrderType = o.OrderType,
                OrderStatus = o.OrderStatus,
                NumberOfGuests = o.NumberOfGuests,
                SubTotal = o.SubTotal,
                DiscountAmount = o.DiscountAmount,
                TaxAmount = o.TaxAmount,
                DeliveryPrice = o.DeliveryPrice,
                TotalAmount = o.TotalAmount,
                Note = o.Note,
                CreatedAt = o.CreatedAt,
                ClosedAt = o.ClosedAt,
                Customer = new UserInfoDto
                {
                    UserId = o.User.UserId,
                    Fullname = o.User.Fullname,
                    Phone = o.User.Phone,
                    Email = o.User.Email
                },
                ServedBy = o.ServedByNavigation == null ? null : new StaffInfoDto
                {
                    UserId = o.ServedByNavigation.UserId,
                    Fullname = o.ServedByNavigation.User.Fullname
                },
                Delivery = o.Delivery == null ? null : new DeliveryDto
                {
                    DeliveryId = o.Delivery.DeliveryId,
                    RecipientName = o.Delivery.RecipientName,
                    RecipientPhone = o.Delivery.RecipientPhone,
                    Address = o.Delivery.Address,
                    DeliveryStatus = o.Delivery.DeliveryStatus,
                    DeliveryFee = o.Delivery.DeliveryFee,
                    EstimatedDeliveryTime = o.Delivery.EstimatedDeliveryTime,
                    ActualDeliveryTime = o.Delivery.ActualDeliveryTime
                },
                Payments = o.Payments.Select(p => new PaymentDto
                {
                    PaymentId = p.PaymentId,
                    PaymentMethod = p.PaymentMethod,
                    PaymentStatus = p.PaymentStatus,
                    Amount = p.Amount,
                    PaidAt = p.PaidAt
                }).ToList(),
                Items = o.OrderItems.Select(i => new OrderItemDetailDto
                {
                    OrderItemId = i.OrderItemId,
                    ItemType = i.Food != null ? "Food"
                              : i.Combo != null ? "Combo"
                              : "Buffet",
                    ItemName = i.Food?.Name ?? i.Combo?.Name ?? i.Buffet?.Name ?? "",
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    Subtotal = i.Subtotal,
                    Status = i.Status,
                    OpeningTime = i.OpeningTime,
                    ServedTime = i.ServedTime
                }).ToList()
            };
        }
    }
}
