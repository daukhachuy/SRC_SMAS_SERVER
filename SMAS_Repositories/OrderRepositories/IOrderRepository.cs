using SMAS_BusinessObject.DTOs.OrderDTO;
using SMAS_BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Repositories.OrderRepositories
{
    public interface IOrderRepository
    {
        Task<IEnumerable<OrderListResponseDTO>> GetOrdersByUserAndStatusAsync(OrderListStatusRequest request, int userid);

        Task<OrderDeliveryResponse> CreateOrderDeliveryAsync(CreateOrderDeliveryRequest request ,int userid);

        Task<Order?> GetOrderByIdAsync(int orderId);

        Task<bool> UpdateOrderStatusAsync(int orderId, string orderStatus);
        Task<bool> AddPaymentAndUpdateOrderStatusAsync(int orderId, string orderStatus, Payment payment);
        Task<List<OrderListResponseDTO>> GetAllActiveOrderAsync();
        Task<List<OrderListResponseDTO>> GetAllOrderCompleteAndCancelAsync();
        Task<List<OrderListResponseDTO>> GetAllActiveOrderByOrderTypeAsync(string orderType);

        Task<OrderListResponseDTO?> GetOrderDetailByOrderCodeAsync(string orderCode);
        Task<List<OrderListResponseDTO>> GetAllOrderCompleteAndCancelByOrderTypeAsync(string orderType);
        Task<List<OrderListResponseDTO>> GetAllOrderCompleteAndCancelByCustomerIdAsync(int customerId);
        Task<AddOrderItemResponse> AddOrderItemByOrderCodeAsync(string orderCode, AddOrderItemRequest request);

        Task<bool> UpdateOrderDeliveryFailedAtAsync(FailDeliveryRequestDTO request);
        Task<Reservation?> GetReservationByCodeAsync(string reservationCode);
        Task<bool> HasActiveOrderByReservationIdAsync(int reservationId);
        Task<User?> GetUserByPhoneAsync(string phone);
        Task<User?> GetUserByEmailAsync(string email);
        Task<bool> TableExistsAsync(int tableId);
        Task<bool> IsTableOccupiedAsync(int tableId);
        Task<Food?> GetFoodByIdForOrderAsync(int foodId);
        Task<Buffet?> GetBuffetByIdForOrderAsync(int buffetId);
        Task<Combo?> GetComboByIdForOrderAsync(int comboId);
        Task CreateInHouseOrderAsync(Order order, List<OrderItem> items, List<TableOrder> tableOrders, Reservation? reservationToUpdate);
        Task<List<OrderListResponseDTO>> GetAllOrderPreparingByWaiterIdAsync(int userId);
        Task<List<OrderListResponseDTO>> GetAllOrderDeliveryByWaiterIdAsync(int userId);
        Task<List<OrderListResponseDTO>> GetAllOrderHistoryByWaiterIdInSevenDayAsync(int userId);
    }
}
