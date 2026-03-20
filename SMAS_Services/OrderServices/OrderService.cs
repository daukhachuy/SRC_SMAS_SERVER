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

        // API 0: lookup thông tin khách trước khi tạo order
        public async Task<OrderLookupResponseDto> LookupOrderAsync(OrderLookupRequestDto request)
        {
            var type = request.Type?.Trim();

            if (string.IsNullOrWhiteSpace(type))
                throw new ArgumentException("Invalid type");

            if (string.Equals(type, "Reservation", StringComparison.OrdinalIgnoreCase))
            {
                var keyword = request.Keyword?.Trim();
                if (string.IsNullOrWhiteSpace(keyword))
                    throw new ArgumentException("Reservation code is required");

                var reservation = await _orderRepository.GetReservationByCodeAsync(keyword);
                if (reservation == null)
                    throw new KeyNotFoundException("Reservation not found");

                if (!string.Equals(reservation.Status, "Confirmed", StringComparison.OrdinalIgnoreCase))
                    throw new ArgumentException("Reservation is not confirmed");

                var hasActiveOrder = await _orderRepository.HasActiveOrderByReservationIdAsync(reservation.ReservationId);
                if (hasActiveOrder)
                    throw new ArgumentException("Reservation already has an active order");

                return new OrderLookupResponseDto
                {
                    Type = "Reservation",
                    UserId = reservation.UserId,
                    FullName = reservation.User?.Fullname,
                    Phone = reservation.User?.Phone,
                    ReservationCode = reservation.ReservationCode,
                    ReservationId = reservation.ReservationId,
                    NumberOfGuests = reservation.NumberOfGuests,
                    ReservationDate = reservation.ReservationDate,
                    ReservationTime = reservation.ReservationTime,
                    OrderType = "DineIn"
                };
            }

            if (string.Equals(type, "Member", StringComparison.OrdinalIgnoreCase))
            {
                var keyword = request.Keyword?.Trim();
                if (string.IsNullOrWhiteSpace(keyword))
                    throw new ArgumentException("Phone or email is required");

                var user = await _orderRepository.GetUserByPhoneAsync(keyword);
                if (user == null)
                    user = await _orderRepository.GetUserByEmailAsync(keyword);

                if (user == null)
                    throw new KeyNotFoundException("Customer not found");

                if (!string.Equals(user.Role, "Customer", StringComparison.OrdinalIgnoreCase))
                    throw new ArgumentException("Account is not a customer account");

                return new OrderLookupResponseDto
                {
                    Type = "Member",
                    UserId = user.UserId,
                    FullName = user.Fullname,
                    Phone = user.Phone,
                    OrderType = null
                };
            }

            if (string.Equals(type, "Guest", StringComparison.OrdinalIgnoreCase))
            {
                return new OrderLookupResponseDto
                {
                    Type = "Guest"
                };
            }

            throw new ArgumentException("Invalid type");
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

            var now = DateTime.UtcNow;
            var tableOrders = await ValidateAndBuildTableOrders(request.TableIds);
            var builtItems = await BuildOrderItems(request.OrderItems);
            var subTotal = builtItems.Sum(x => x.Subtotal);

            var orderItems = builtItems.Select(b => new OrderItem
            {
                FoodId = b.Request.FoodId,
                BuffetId = b.Request.BuffetId,
                ComboId = b.Request.ComboId,
                Quantity = b.Request.Quantity,
                UnitPrice = b.UnitPrice,
                Subtotal = b.Subtotal,
                Status = "Pending",
                Note = b.Request.Note,
                OpeningTime = now,
                ServedTime = null
            }).ToList();

            var order = new Order
            {
                OrderCode = GenerateOrderCode(now),
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

            await _orderRepository.CreateInHouseOrderAsync(order, orderItems, tableOrders, reservation);

            return MapCreateInHouseOrderResponse(order, request.TableIds!, tableOrders, orderItems);
        }

        public async Task<CreateInHouseOrderResponse> CreateOrderByContactAsync(CreateOrderByContactRequest request, int waiterUserId)
        {
            if (string.IsNullOrWhiteSpace(request.Phone) && string.IsNullOrWhiteSpace(request.Email))
                throw new ArgumentException("Phone or email is required");

            if (!string.Equals(request.OrderType, "DineIn", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(request.OrderType, "Buffet", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Invalid order type");

            User? user = null;

            // ưu tiên tìm theo phone trước
            if (!string.IsNullOrWhiteSpace(request.Phone))
            {
                var phone = request.Phone.Trim();
                user = await _orderRepository.GetUserByPhoneAsync(phone);
            }

            if (user == null && !string.IsNullOrWhiteSpace(request.Email))
            {
                var email = request.Email.Trim();
                user = await _orderRepository.GetUserByEmailAsync(email);
            }

            if (user == null)
                throw new KeyNotFoundException("Customer not found with provided phone/email");

            if (!string.Equals(user.Role, "Customer", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Account is not a customer account");

            var now = DateTime.UtcNow;
            var tableOrders = await ValidateAndBuildTableOrders(request.TableIds);
            var builtItems = await BuildOrderItems(request.OrderItems);

            var subTotal = builtItems.Sum(x => x.Subtotal);

            var orderItems = builtItems.Select(b => new OrderItem
            {
                FoodId = b.Request.FoodId,
                BuffetId = b.Request.BuffetId,
                ComboId = b.Request.ComboId,
                Quantity = b.Request.Quantity,
                UnitPrice = b.UnitPrice,
                Subtotal = b.Subtotal,
                Status = "Pending",
                Note = b.Request.Note,
                OpeningTime = now,
                ServedTime = null
            }).ToList();

            var order = new Order
            {
                OrderCode = GenerateOrderCode(now),
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

            await _orderRepository.CreateInHouseOrderAsync(order, orderItems, tableOrders, null);

            return MapCreateInHouseOrderResponse(order, request.TableIds!, tableOrders, orderItems);
        }

        public async Task<CreateInHouseOrderResponse> CreateGuestOrderAsync(CreateGuestOrderRequest request, int waiterUserId)
        {
            if (!string.Equals(request.OrderType, "DineIn", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(request.OrderType, "Buffet", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Invalid order type");

            var now = DateTime.UtcNow;
            var tableOrders = await ValidateAndBuildTableOrders(request.TableIds);
            var builtItems = await BuildOrderItems(request.OrderItems);

            var subTotal = builtItems.Sum(x => x.Subtotal);

            var orderItems = builtItems.Select(b => new OrderItem
            {
                FoodId = b.Request.FoodId,
                BuffetId = b.Request.BuffetId,
                ComboId = b.Request.ComboId,
                Quantity = b.Request.Quantity,
                UnitPrice = b.UnitPrice,
                Subtotal = b.Subtotal,
                Status = "Pending",
                Note = b.Request.Note,
                OpeningTime = now,
                ServedTime = null
            }).ToList();

            var order = new Order
            {
                OrderCode = GenerateOrderCode(now),
                UserId = waiterUserId,
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

            await _orderRepository.CreateInHouseOrderAsync(order, orderItems, tableOrders, null);

            return MapCreateInHouseOrderResponse(order, request.TableIds!, tableOrders, orderItems);
        }

        // Shared helper: validate/occupancy and build TableOrder list (main table = tableIds[0])
        private async Task<List<TableOrder>> ValidateAndBuildTableOrders(List<int>? tableIds)
        {
            if (tableIds == null || tableIds.Count == 0)
                throw new ArgumentException("At least one table is required");

            var now = DateTime.UtcNow;
            var tableOrders = new List<TableOrder>();

            foreach (var tableId in tableIds)
            {
                var tableExists = await _orderRepository.TableExistsAsync(tableId);
                if (!tableExists)
                    throw new KeyNotFoundException($"Table {tableId} not found");

                var isOccupied = await _orderRepository.IsTableOccupiedAsync(tableId);
                if (isOccupied)
                    throw new ArgumentException($"Table {tableId} is currently occupied");

                tableOrders.Add(new TableOrder
                {
                    TableId = tableId,
                    OrderId = 0, // will be assigned after order is saved
                    IsMainTable = tableOrders.Count == 0,
                    JoinedAt = now,
                    LeftAt = null
                });
            }

            return tableOrders;
        }

        private sealed class BuiltOrderItem
        {
            public required OrderItemRequest Request { get; init; }
            public decimal UnitPrice { get; init; }
            public decimal Subtotal { get; init; }
        }

        // Shared helper: compute unitPrice/subtotal for each item
        private async Task<List<BuiltOrderItem>> BuildOrderItems(List<OrderItemRequest>? items)
        {
            var built = new List<BuiltOrderItem>();
            if (items == null || !items.Any())
                return built;

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
                    var combo = await _orderRepository.GetComboByIdForOrderAsync(item.ComboId!.Value);
                    if (combo == null)
                        throw new KeyNotFoundException("Combo not found");

                    var isExpired = combo.ExpiryDate.HasValue && combo.ExpiryDate.Value < today;
                    if (combo.IsAvailable != true || isExpired)
                        throw new ArgumentException($"Combo {combo.Name} is not available");

                    unitPrice = combo.DiscountPercent != null
                        ? combo.Price * (1 - combo.DiscountPercent.Value / 100)
                        : combo.Price;
                }

                built.Add(new BuiltOrderItem
                {
                    Request = item,
                    UnitPrice = unitPrice,
                    Subtotal = unitPrice * item.Quantity
                });
            }

            return built;
        }

        private static string GenerateOrderCode(DateTime now)
        {
            var random = Random.Shared.Next(1000, 10000);
            return $"ORD-{now:yyyyMMddHHmmss}-{random}";
        }

        private static CreateInHouseOrderResponse MapCreateInHouseOrderResponse(
            Order order,
            List<int> tableIds,
            List<TableOrder> tableOrders,
            List<OrderItem> orderItems)
        {
            var mainTableId = tableIds.First();
            return new CreateInHouseOrderResponse
            {
                OrderId = order.OrderId,
                OrderCode = order.OrderCode,
                OrderStatus = order.OrderStatus,
                OrderType = order.OrderType,
                TableIds = tableIds,
                MainTableId = mainTableId,
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
