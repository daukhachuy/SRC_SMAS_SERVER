using SMAS_BusinessObject.DTOs.ManagerDTO;
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
        Task<List<OrderListResponseDTO>> GetAllOrderCompleteAndCancelByCustomerIdAsync(int customerId);
        Task<AddOrderItemResponse> AddOrderItemByOrderCodeAsync(string orderCode, AddOrderItemRequest request);
        Task<AddOrderItemResponse> AddOrderItemByTableTokenAsync(string orderCode, AddOrderItemRequest request, string accessToken);
        Task<bool> UpdateOrderDeliveryFailedAtAsync(FailDeliveryRequestDTO request);
        Task<List<OrderListResponseDTO>> GetAllOrderPreparingByWaiterIdAsync(int userId);
        Task<List<OrderListResponseDTO>> GetAllOrderDeliveryByWaiterIdAsync(int userId);
        Task<List<OrderListResponseDTO>> GetAllOrderHistoryByWaiterIdInSevenDayAsync(int userId);
        Task<CreateInHouseOrderResponse> CreateOrderByReservationAsync(CreateOrderByReservationRequest request, int waiterUserId);
        Task<CreateInHouseOrderResponse> CreateOrderByContactAsync(CreateOrderByContactRequest request, int waiterUserId);
        Task<CreateInHouseOrderResponse> CreateGuestOrderAsync(CreateGuestOrderRequest request, int waiterUserId);

        Task<OrderLookupResponseDto> LookupOrderAsync(OrderLookupRequestDto request);

        Task<(bool status , string message)> ChooseAssignedStaffbyOrderAsync(ChooseAssignedStaffRequestDTO request);

        Task<(bool status, string message)> ChangeStatusDeliveryAsync(string request);

        Task<(bool status, string message)> DeleteOrderDeliveryByDeliveryCodeAsync(string request , string dto);
        Task<(bool Success, string? ErrorCode, object? Data)> GetMenuForSessionAsync(string accessToken, string? type, int? categoryId, string? keyword);
        Task<(bool Success, string? ErrorCode, object? Data)> GetCurrentOrderBySessionAsync(string accessToken);
    }
}
