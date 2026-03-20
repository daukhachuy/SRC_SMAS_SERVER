using SMAS_BusinessObject.DTOs.OrderDTO;
using SMAS_BusinessObject.Models;
using SMAS_Repositories.OrderRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Services.OrderServices
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<IEnumerable<OrderListResponseDTO>> GetOrdersByUserAndStatusAsync(OrderListStatusRequest request, int userid)
        {
            return await _orderRepository.GetOrdersByUserAndStatusAsync(request, userid);
        }

        public async Task<OrderDeliveryResponse> CreateOrderDeliveryAsync(CreateOrderDeliveryRequest request , int userid)
        {
            return await _orderRepository.CreateOrderDeliveryAsync(request, userid);
        }

        public async Task<List<OrderListResponseDTO>> GetAllActiveOrderAsync()
        {
            return await _orderRepository.GetAllActiveOrderAsync();
        }
        public async Task<List<OrderListResponseDTO>> GetAllOrderCompleteAndCancelAsync()
        {
            return await _orderRepository.GetAllOrderCompleteAndCancelAsync();
        }
        public async Task<List<OrderListResponseDTO>> GetAllOrderCompleteAndCancelByCustomerIdAsync(int customerId)
        {
            if (customerId <= 0)
                throw new ArgumentException("CustomerId không hợp lệ.", nameof(customerId));

            return await _orderRepository.GetAllOrderCompleteAndCancelByCustomerIdAsync(customerId);
        }
        public async Task<List<OrderListResponseDTO>> GetAllActiveOrderByOrderTypeAsync(string orderType)
        {
            return await _orderRepository.GetAllActiveOrderByOrderTypeAsync(orderType);
        }

        public async Task<OrderListResponseDTO?> GetOrderDetailByOrderCodeAsync(string orderCode)
        {
            if (string.IsNullOrWhiteSpace(orderCode))
                throw new ArgumentException("Order code không được để trống.", nameof(orderCode));

            return await _orderRepository.GetOrderDetailByOrderCodeAsync(orderCode);
        }
        public async Task<List<OrderListResponseDTO>> GetAllOrderCompleteAndCancelByOrderTypeAsync(string orderType)
        {
            return await _orderRepository.GetAllOrderCompleteAndCancelByOrderTypeAsync(orderType);
        }
        public async Task<AddOrderItemResponse> AddOrderItemByOrderCodeAsync(string orderCode, AddOrderItemRequest request)
        {
            if (string.IsNullOrWhiteSpace(orderCode))
                return new AddOrderItemResponse { Success = false, Message = "Order code không được để trống." };

            return await _orderRepository.AddOrderItemByOrderCodeAsync(orderCode, request);
        }

        public async Task<bool> UpdateOrderDeliveryFailedAtAsync(FailDeliveryRequestDTO request)
        {
            return await _orderRepository.UpdateOrderDeliveryFailedAtAsync(request);
        }
        public async Task<List<OrderListResponseDTO>> GetAllOrderPreparingByWaiterIdAsync(int userId)
        {
            if (userId <= 0)
                throw new ArgumentException("WaiterId không hợp lệ.", nameof(userId));

            return await _orderRepository.GetAllOrderPreparingByWaiterIdAsync(userId);
        }

        public async Task<List<OrderListResponseDTO>> GetAllOrderDeliveryByWaiterIdAsync(int userId)
        {
            if (userId <= 0)
                throw new ArgumentException("WaiterId không hợp lệ.", nameof(userId));

            return await _orderRepository.GetAllOrderDeliveryByWaiterIdAsync(userId);
        }
        public async Task<List<OrderListResponseDTO>> GetAllOrderHistoryByWaiterIdInSevenDayAsync(int userId)
        {
            if (userId <= 0)
                throw new ArgumentException("UserId không hợp lệ.", nameof(userId));

            return await _orderRepository.GetAllOrderHistoryByWaiterIdInSevenDayAsync(userId);
        }
    }
}
