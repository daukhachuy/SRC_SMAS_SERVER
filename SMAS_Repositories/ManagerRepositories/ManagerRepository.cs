using SMAS_BusinessObject.DTOs.ManagerDTO;
using SMAS_BusinessObject.DTOs.OrderDTO;
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
