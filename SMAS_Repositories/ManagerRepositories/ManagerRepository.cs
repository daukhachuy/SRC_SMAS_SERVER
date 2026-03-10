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

        public ManagerRepository(ManagerDAO managerDAO)
        {
            _managerDAO = managerDAO;
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

        public async Task<RevenueWeekResponseDTO> GetRevenuePreviousSevenDaysAsync()
        {
            var (startDate, endDate, totalRevenue) = await _managerDAO.GetRevenuePreviousSevenDaysAsync();
            return new RevenueWeekResponseDTO
            {
                StartDate = startDate,
                EndDate = endDate,
                TotalRevenue = totalRevenue
            };
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
                CheckOutTime = ws.CheckOutTime
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
