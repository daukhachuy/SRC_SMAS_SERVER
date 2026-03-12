using SMAS_BusinessObject.DTOs.OrderDTO;
using SMAS_BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Services.OrderServices
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderListResponseDTO>> GetOrdersByUserAndStatusAsync(OrderListStatusRequest request , int userid);
        Task<OrderDeliveryResponse> CreateOrderDeliveryAsync(CreateOrderDeliveryRequest request , int userid);
        Task<List<OrderListResponseDTO>> GetAllActiveOrderAsync();
        Task<List<OrderListResponseDTO>> GetAllOrderCompleteAndCancelAsync();
        Task<List<OrderListResponseDTO>> GetAllActiveOrderByOrderTypeAsync(string orderType);
        Task<OrderListResponseDTO?> GetOrderDetailByOrderCodeAsync(string orderCode);
        Task<List<OrderListResponseDTO>> GetAllOrderCompleteAndCancelByOrderTypeAsync(string orderType);
        Task<AddOrderItemResponse> AddOrderItemByOrderCodeAsync(string orderCode, AddOrderItemRequest request);

    }
}
