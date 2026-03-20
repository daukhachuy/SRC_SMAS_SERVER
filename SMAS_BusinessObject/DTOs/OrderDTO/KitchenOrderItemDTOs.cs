using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SMAS_BusinessObject.DTOs.OrderDTO
{
    public class KitchenCancelOrderItemRequestDTO
    {
        [JsonPropertyName("reason")]
        public string? Reason { get; set; }
    }

    public class KitchenPendingItemDTO
    {
        [JsonPropertyName("orderItemId")]
        public int OrderItemId { get; set; }

        [JsonPropertyName("foodId")]
        public int? FoodId { get; set; }

        [JsonPropertyName("buffetId")]
        public int? BuffetId { get; set; }

        [JsonPropertyName("comboId")]
        public int? ComboId { get; set; }

        [JsonPropertyName("itemName")]
        public string ItemName { get; set; } = string.Empty;

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }

        [JsonPropertyName("note")]
        public string? Note { get; set; }

        [JsonPropertyName("openingTime")]
        public DateTime? OpeningTime { get; set; }
    }

    public class KitchenPendingOrderDTO
    {
        [JsonPropertyName("orderId")]
        public int OrderId { get; set; }

        [JsonPropertyName("orderCode")]
        public string OrderCode { get; set; } = string.Empty;

        [JsonPropertyName("tableId")]
        public int TableId { get; set; }

        [JsonPropertyName("orderCreatedAt")]
        public DateTime OrderCreatedAt { get; set; }

        [JsonPropertyName("pendingItems")]
        public List<KitchenPendingItemDTO> PendingItems { get; set; } = new();
    }

    public class KitchenOrderItemPreparingResponseDTO
    {
        [JsonPropertyName("orderItemId")]
        public int OrderItemId { get; set; }

        [JsonPropertyName("orderId")]
        public int OrderId { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; } = "Preparing";

        [JsonPropertyName("updatedAt")]
        public DateTime UpdatedAt { get; set; }
    }

    public class KitchenOrderItemReadyResponseDTO
    {
        [JsonPropertyName("orderItemId")]
        public int OrderItemId { get; set; }

        [JsonPropertyName("orderId")]
        public int OrderId { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; } = "Ready";

        [JsonPropertyName("servedTime")]
        public DateTime ServedTime { get; set; }
    }

    public class KitchenOrderItemCancelledResponseDTO
    {
        [JsonPropertyName("orderItemId")]
        public int OrderItemId { get; set; }

        [JsonPropertyName("orderId")]
        public int OrderId { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; } = "Cancelled";

        [JsonPropertyName("subtotal")]
        public decimal Subtotal { get; set; }

        [JsonPropertyName("note")]
        public string? Note { get; set; }

        [JsonPropertyName("newOrderSubTotal")]
        public decimal NewOrderSubTotal { get; set; }

        [JsonPropertyName("newOrderTotalAmount")]
        public decimal NewOrderTotalAmount { get; set; }
    }

    public class KitchenUpdatedPreparingItemDTO
    {
        [JsonPropertyName("orderItemId")]
        public int OrderItemId { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; } = "Preparing";
    }

    public class KitchenUpdatedReadyItemDTO
    {
        [JsonPropertyName("orderItemId")]
        public int OrderItemId { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; } = "Ready";

        [JsonPropertyName("servedTime")]
        public DateTime ServedTime { get; set; }
    }

    public class KitchenUpdateAllPreparingResponseDTO
    {
        [JsonPropertyName("orderId")]
        public int OrderId { get; set; }

        [JsonPropertyName("orderCode")]
        public string OrderCode { get; set; } = string.Empty;

        [JsonPropertyName("updatedCount")]
        public int UpdatedCount { get; set; }

        [JsonPropertyName("updatedItems")]
        public List<KitchenUpdatedPreparingItemDTO> UpdatedItems { get; set; } = new();
    }

    public class KitchenUpdateAllReadyResponseDTO
    {
        [JsonPropertyName("orderId")]
        public int OrderId { get; set; }

        [JsonPropertyName("orderCode")]
        public string OrderCode { get; set; } = string.Empty;

        [JsonPropertyName("updatedCount")]
        public int UpdatedCount { get; set; }

        [JsonPropertyName("updatedItems")]
        public List<KitchenUpdatedReadyItemDTO> UpdatedItems { get; set; } = new();
    }

    public class KitchenTodayHistoryItemDTO
    {
        [JsonPropertyName("orderItemId")]
        public int OrderItemId { get; set; }

        [JsonPropertyName("orderId")]
        public int OrderId { get; set; }

        [JsonPropertyName("orderCode")]
        public string OrderCode { get; set; } = string.Empty;

        [JsonPropertyName("tableId")]
        public int? TableId { get; set; }

        [JsonPropertyName("foodId")]
        public int? FoodId { get; set; }

        [JsonPropertyName("buffetId")]
        public int? BuffetId { get; set; }

        [JsonPropertyName("comboId")]
        public int? ComboId { get; set; }

        [JsonPropertyName("itemName")]
        public string ItemName { get; set; } = string.Empty;

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }

        [JsonPropertyName("unitPrice")]
        public decimal UnitPrice { get; set; }

        [JsonPropertyName("subtotal")]
        public decimal? Subtotal { get; set; }

        [JsonPropertyName("note")]
        public string? Note { get; set; }

        [JsonPropertyName("openingTime")]
        public DateTime? OpeningTime { get; set; }

        [JsonPropertyName("servedTime")]
        public DateTime? ServedTime { get; set; }
    }

    public class KitchenTodayHistoryResponseDTO
    {
        [JsonPropertyName("date")]
        public string Date { get; set; } = string.Empty;

        [JsonPropertyName("totalItems")]
        public int TotalItems { get; set; }

        [JsonPropertyName("items")]
        public List<KitchenTodayHistoryItemDTO> Items { get; set; } = new();
    }
}

