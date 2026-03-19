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

        public async Task<CreateInHouseOrderResponse> CreateOrderByReservationAsync(CreateOrderByReservationRequest request, int waiterUserId)
        {
            var reservationCode = request.ReservationCode?.Trim();
            if (string.IsNullOrWhiteSpace(reservationCode))
                throw new ArgumentException("Reservation code is required");

            var reservation = await _orderRepository.GetReservationByCodeAsync(reservationCode);
            if (reservation == null)
                throw new KeyNotFoundException("Reservation not found");

            if (!string.Equals(reservation.Status, "Confirmed", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Reservation is not confirmed");

            var hasActiveOrder = await _orderRepository.HasActiveOrderByReservationIdAsync(reservation.ReservationId);
            if (hasActiveOrder)
                throw new ArgumentException("Reservation already has an active order");

            await ValidateTableAvailability(request.TableId);
            var builtItems = await BuildOrderItems(request.OrderItems ?? new List<OrderItemRequest>());
            var subTotal = builtItems.Sum(x => x.Subtotal ?? 0m);
            var now = DateTime.UtcNow;

            var order = new Order
            {
                OrderCode = GenerateOrderCode(),
                UserId = reservation.UserId,
                ReservationId = reservation.ReservationId,
                BookEventId = null,
                DiscountId = null,
                DeliveryId = null,
                OrderType = "DineIn",
                OrderStatus = "Pending",
                NumberOfGuests = request.NumberOfGuests,
                Note = request.Note,
                ServedBy = waiterUserId,
                SubTotal = subTotal,
                DiscountAmount = null,
                TaxAmount = null,
                DeliveryPrice = null,
                TotalAmount = subTotal,
                CreatedAt = now,
                ClosedAt = null
            };

            await _orderRepository.CreateInHouseOrderAsync(order, builtItems, request.TableId, reservation);

            return MapCreateInHouseOrderResponse(order, request.TableId, builtItems);
        }

        public async Task<CreateInHouseOrderResponse> CreateOrderByContactAsync(CreateOrderByContactRequest request, int waiterUserId)
        {
            if (string.IsNullOrWhiteSpace(request.Phone) && string.IsNullOrWhiteSpace(request.Email))
                throw new ArgumentException("Phone or email is required");

            if (!string.Equals(request.OrderType, "DineIn", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(request.OrderType, "Buffet", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Invalid order type");

            User? user;
            if (!string.IsNullOrWhiteSpace(request.Phone))
            {
                var phone = request.Phone.Trim();
                user = await _orderRepository.GetUserByPhoneAsync(phone);
            }
            else
            {
                var email = request.Email!.Trim();
                user = await _orderRepository.GetUserByEmailAsync(email);
            }

            if (user == null)
                throw new KeyNotFoundException("Customer not found with provided phone/email");

            await ValidateTableAvailability(request.TableId);
            var builtItems = await BuildOrderItems(request.OrderItems ?? new List<OrderItemRequest>());
            var subTotal = builtItems.Sum(x => x.Subtotal ?? 0m);
            var now = DateTime.UtcNow;

            var order = new Order
            {
                OrderCode = GenerateOrderCode(),
                UserId = user.UserId,
                ReservationId = null,
                BookEventId = null,
                DiscountId = null,
                DeliveryId = null,
                OrderType = request.OrderType,
                OrderStatus = "Pending",
                NumberOfGuests = request.NumberOfGuests,
                Note = request.Note,
                ServedBy = waiterUserId,
                SubTotal = subTotal,
                DiscountAmount = null,
                TaxAmount = null,
                DeliveryPrice = null,
                TotalAmount = subTotal,
                CreatedAt = now,
                ClosedAt = null
            };

            await _orderRepository.CreateInHouseOrderAsync(order, builtItems, request.TableId, null);

            return MapCreateInHouseOrderResponse(order, request.TableId, builtItems);
        }

        public async Task<CreateInHouseOrderResponse> CreateGuestOrderAsync(CreateGuestOrderRequest request, int customerUserId)
        {
            await ValidateTableAvailability(request.TableId);
            var builtItems = await BuildOrderItems(request.OrderItems ?? new List<OrderItemRequest>());
            var subTotal = builtItems.Sum(x => x.Subtotal ?? 0m);
            var now = DateTime.UtcNow;

            var order = new Order
            {
                OrderCode = GenerateOrderCode(),
                UserId = customerUserId,
                ReservationId = null,
                BookEventId = null,
                DiscountId = null,
                DeliveryId = null,
                OrderType = "DineIn",
                OrderStatus = "Pending",
                NumberOfGuests = request.NumberOfGuests,
                Note = request.Note,
                ServedBy = null,
                SubTotal = subTotal,
                DiscountAmount = null,
                TaxAmount = null,
                DeliveryPrice = null,
                TotalAmount = subTotal,
                CreatedAt = now,
                ClosedAt = null
            };

            await _orderRepository.CreateInHouseOrderAsync(order, builtItems, request.TableId, null);

            return MapCreateInHouseOrderResponse(order, request.TableId, builtItems);
        }

        private async Task ValidateTableAvailability(int tableId)
        {
            var tableExists = await _orderRepository.TableExistsAsync(tableId);
            if (!tableExists)
                throw new KeyNotFoundException("Table not found");

            var isOccupied = await _orderRepository.IsTableOccupiedAsync(tableId);

            if (isOccupied)
                throw new ArgumentException("Table is currently occupied");
        }

        private async Task<List<OrderItem>> BuildOrderItems(List<OrderItemRequest> items)
        {
            var orderItems = new List<OrderItem>();
            if (items == null || !items.Any())
                return orderItems;

            var today = DateOnly.FromDateTime(DateTime.Today);
            foreach (var item in items)
            {
                var selectedCount = 0;
                if (item.FoodId.HasValue) selectedCount++;
                if (item.BuffetId.HasValue) selectedCount++;
                if (item.ComboId.HasValue) selectedCount++;

                if (selectedCount != 1)
                    throw new ArgumentException("Each order item must have exactly one of foodId, buffetId, or comboId");

                if (item.Quantity <= 0)
                    throw new ArgumentException("Quantity must be greater than 0");

                decimal unitPrice;
                if (item.FoodId.HasValue)
                {
                    var food = await _orderRepository.GetFoodByIdForOrderAsync(item.FoodId.Value);
                    if (food == null)
                        throw new KeyNotFoundException("Food not found");
                    if (food.IsAvailable != true)
                        throw new ArgumentException($"Food {food.Name} is not available");

                    unitPrice = food.PromotionalPrice ?? food.Price;
                }
                else if (item.BuffetId.HasValue)
                {
                    var buffet = await _orderRepository.GetBuffetByIdForOrderAsync(item.BuffetId.Value);
                    if (buffet == null)
                        throw new KeyNotFoundException("Buffet not found");
                    if (buffet.IsAvailable != true)
                        throw new ArgumentException($"Buffet {buffet.Name} is not available");

                    unitPrice = buffet.MainPrice;
                }
                else
                {
                    var combo = await _orderRepository.GetComboByIdForOrderAsync(item.ComboId.Value);
                    if (combo == null)
                        throw new KeyNotFoundException("Combo not found");
                    if (combo.IsAvailable != true || (combo.ExpiryDate.HasValue && combo.ExpiryDate.Value < today))
                        throw new ArgumentException($"Combo {combo.Name} is not available");

                    unitPrice = combo.DiscountPercent.HasValue
                        ? combo.Price * (1 - combo.DiscountPercent.Value / 100)
                        : combo.Price;
                }

                orderItems.Add(new OrderItem
                {
                    FoodId = item.FoodId,
                    BuffetId = item.BuffetId,
                    ComboId = item.ComboId,
                    Quantity = item.Quantity,
                    UnitPrice = unitPrice,
                    Subtotal = unitPrice * item.Quantity,
                    Status = "Pending",
                    Note = item.Note,
                    OpeningTime = DateTime.UtcNow,
                    ServedTime = null
                });
            }

            return orderItems;
        }

        private static string GenerateOrderCode()
        {
            var random = Random.Shared.Next(1000, 10000);
            return $"ORD-{DateTime.UtcNow:yyyyMMddHHmmss}-{random}";
        }

        private static CreateInHouseOrderResponse MapCreateInHouseOrderResponse(Order order, int tableId, List<OrderItem> orderItems)
        {
            return new CreateInHouseOrderResponse
            {
                OrderId = order.OrderId,
                OrderCode = order.OrderCode,
                OrderStatus = order.OrderStatus,
                OrderType = order.OrderType,
                TableId = tableId,
                NumberOfGuests = order.NumberOfGuests ?? 0,
                SubTotal = order.SubTotal ?? 0,
                TotalAmount = order.TotalAmount,
                CreatedAt = order.CreatedAt,
                OrderItems = orderItems.Select(x => new CreateInHouseOrderItemResponse
                {
                    OrderItemId = x.OrderItemId,
                    FoodId = x.FoodId,
                    BuffetId = x.BuffetId,
                    ComboId = x.ComboId,
                    Quantity = x.Quantity,
                    UnitPrice = x.UnitPrice,
                    Subtotal = x.Subtotal ?? 0,
                    Status = x.Status,
                    Note = x.Note
                }).ToList()
            };
        }
    }
}
