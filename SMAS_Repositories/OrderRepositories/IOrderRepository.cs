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
        Task<AddOrderItemResponse> AddOrderItemByOrderCodeAsync(string orderCode, AddOrderItemRequest request);

        Task<bool> UpdateOrderDeliveryFailedAtAsync(FailDeliveryRequestDTO request);
    }
}
