using SMAS_BusinessObject.DTOs.OrderDTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SMAS_Services.OrderItemServices
{
    public interface IOrderItemService
    {
        Task<List<KitchenPendingOrderDTO>> GetActiveOrdersWithPendingItemsAsync();

        Task<KitchenOrderItemPreparingResponseDTO> PatchUpdateStatusOrderItemPreparingAsync(int orderItemId);

        Task<KitchenOrderItemReadyResponseDTO> PatchUpdateStatusOrderItemReadyAsync(int orderItemId);

        Task<KitchenOrderItemReadyResponseDTO> PatchUpdateStatusOrderItemServedAsync(int orderItemId);

        Task<KitchenOrderItemCancelledResponseDTO> PostUpdateStatusOrderItemCancelledAsync(int orderItemId, KitchenCancelOrderItemRequestDTO request);

        Task<KitchenUpdateAllPreparingResponseDTO> PatchUpdateStatusAllOrderItemPreparingAsync(int orderId);

        Task<KitchenUpdateAllReadyResponseDTO> PatchUpdateStatusAllOrderItemReadyAsync(int orderId);

        Task<KitchenTodayHistoryResponseDTO> GetAllOrderItemsHistoryTodayAsync(int? orderId);

        Task<(bool status, string message)> AddOrderItemByOrderCodeAsync(string orderCode, List<AddOrderItemRequest> request);
    }
}

