using Microsoft.AspNetCore.SignalR;
using SMAS_Services.Realtime;

namespace SMAS_API.Hubs
{
    public class KitchenNotifier : IKitchenNotifier
    {
        private readonly IHubContext<KitchenHub> _hubContext;

        public KitchenNotifier(IHubContext<KitchenHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotifyOrderItemStatusChanged(int orderId, int orderItemId, string newStatus)
        {
            var payload = new { orderId, orderItemId, status = newStatus, updatedAt = DateTime.UtcNow };

            await _hubContext.Clients.Groups("Kitchen", "Waiter")
                .SendAsync("OrderItemStatusChanged", payload);
        }

        public async Task NotifyAllItemsStatusChanged(int orderId, string newStatus, List<int> orderItemIds)
        {
            var payload = new { orderId, orderItemIds, status = newStatus, updatedAt = DateTime.UtcNow };

            await _hubContext.Clients.Groups("Kitchen", "Waiter")
                .SendAsync("AllItemsStatusChanged", payload);
        }

        public async Task NotifyNewOrderItems(int orderId, string orderCode)
        {
            var payload = new { orderId, orderCode, createdAt = DateTime.UtcNow };

            await _hubContext.Clients.Group("Kitchen")
                .SendAsync("NewOrderItems", payload);
        }
    }
}
