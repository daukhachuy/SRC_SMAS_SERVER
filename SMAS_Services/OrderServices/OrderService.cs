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
    }
}
