namespace SMAS_Services.Realtime
{
    public interface IKitchenNotifier
    {
        Task NotifyOrderItemStatusChanged(int orderId, int orderItemId, string newStatus);

        Task NotifyAllItemsStatusChanged(int orderId, string newStatus, List<int> orderItemIds);

        Task NotifyNewOrderItems(int orderId, string orderCode);
    }
}
