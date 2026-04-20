using Azure.Core;
using CloudinaryDotNet.Actions;
using SMAS_BusinessObject.DTOs.OrderDTO;
using SMAS_BusinessObject.Models;
using SMAS_Repositories.OrderRepositories;
using SMAS_Services.NotificationServices;
using SMAS_Services.TableService;
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
        private readonly ITableService _tableService;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly INotificationService _notificationService;
        public OrderService(IOrderRepository orderRepository,IOrderItemRepository orderItemRepository, ITableService tableService, INotificationService notificationService)
        {
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _tableService = tableService;
            _notificationService = notificationService;
        }
        public async Task<IEnumerable<OrderListResponseDTO>> GetOrdersByUserAndStatusAsync(OrderListStatusRequest request, int userid)
        {
            return await _orderRepository.GetOrdersByUserAndStatusAsync(request, userid);
        }

        public async Task<OrderDeliveryResponse> CreateOrderDeliveryAsync(CreateOrderDeliveryRequest request, int userid)
        {
            var result = await _orderRepository.CreateOrderDeliveryAsync(request, userid);

            if (result.Success)
            {
                await _notificationService.CreateAutoNotificationAsync(
                    userId: userid,
                    senderId: null,
                    title: "Đặt hàng thành công",
                    content: $"Đơn hàng {result.OrderCode} đã được tạo thành công. " +
                             $"Chúng tôi sẽ xử lý đơn hàng của bạn sớm nhất!",
                    type: "Order",
                    severity: "Information"
                );
            }

            return result;
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
        public async Task<AddOrderItemResponse> AddOrderItemByTableTokenAsync(
       string orderCode,
       AddOrderItemRequest request,
       string accessToken)
        {
            if (string.IsNullOrWhiteSpace(orderCode) || request == null)
                return new AddOrderItemResponse { Success = false, Message = "Dữ liệu không hợp lệ." };

            // Validate Token từ Table
            var validateResult = _tableService.ValidateAccessToken(accessToken);
            if (!validateResult.Valid)
            {
                string msg = validateResult.ErrorCode switch
                {
                    "INVALID_QR_TOKEN" => "Token bàn không hợp lệ. Vui lòng quét QR lại.",
                    "SESSION_NOT_ACTIVE" or "TABLE_CLOSED" => "Phiên bàn đã đóng.",
                    "SESSION_EXPIRED" => "Phiên bàn đã hết hạn.",
                    _ => "Token không hợp lệ."
                };
                return new AddOrderItemResponse { Success = false, Message = msg };
            }
            string tableCodeFromToken = validateResult.TableCode ?? "";
            // Kiểm tra bàn có khớp với order không
            var order = await _orderRepository.GetOrderByIdNoTrackingAsync(orderCode); // hoặc GetOrderByCodeNoTrackingAsync
            if (order == null)
                return new AddOrderItemResponse { Success = false, Message = "Không tìm thấy đơn hàng." };

            var activeTable = order.TableOrders?.FirstOrDefault(t => t.LeftAt == null);
            //if (activeTable == null || activeTable.TableId.ToString() != validateResult.TableCode)
            //    return new AddOrderItemResponse { Success = false, Message = "Bạn chỉ có thể đặt món tại bàn bạn đang ngồi." };

            // Debug log
            Console.WriteLine($"[DEBUG] Token TableCode: {tableCodeFromToken} | Order Tables: {string.Join(",", order.TableOrders?.Select(t => t.TableId) ?? new List<int>())}");

            if (activeTable == null || activeTable.TableId.ToString() != tableCodeFromToken)
            {
                return new AddOrderItemResponse
                {
                    Success = false,
                    Message = $"Bạn chỉ có thể đặt món tại bàn bạn đang ngồi. (Token: bàn {tableCodeFromToken})"
                };
            }
            // Gọi Repository để thêm món
            var result = await _orderItemRepository.AddOrderItemAsync(orderCode, request);

            // === THÊM: Notify Kitchen khi khách đặt món qua QR ===
            if (result.Success)
            {
                try
                {
                    var kitchenUsers = await _orderRepository.GetUsersByRoleAsync("Kitchen");
                    foreach (var kitchenUser in kitchenUsers)
                    {
                        await _notificationService.CreateAutoNotificationAsync(
                            userId: kitchenUser.UserId,
                            senderId: null,
                            title: "Có món mới từ khách (QR)",
                            content: $"Đơn hàng {orderCode} - Bàn {tableCodeFromToken} vừa thêm món mới. Vui lòng kiểm tra!",
                            type: "Order",
                            severity: "Information"
                        );
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] QR notification failed: {ex.Message}");
                }
            }

            return result;

        }

        public async Task<bool> UpdateOrderDeliveryFailedAtAsync(FailDeliveryRequestDTO request)
        {
            var result = await _orderRepository.UpdateOrderDeliveryFailedAtAsync(request);

            if (result)
            {
                var order = await _orderRepository.GetOrderByIdAsync(request.orderId);
                if (order != null)
                {
                    await _notificationService.CreateAutoNotificationAsync(
                        userId: order.UserId,
                        senderId: null,
                        title: "Giao hàng thất bại",
                        content: $"Đơn hàng {order.OrderCode} giao không thành công. " +
                                 $"Lý do: {request.reason}",
                        type: "Order",
                        severity: "Warning"
                    );
                }
            }

            return result;
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

            var now = DateTime.Now;
            var tableOrders = await ValidateAndBuildTableOrders(request.TableIds);
            var subTotal = 0m;
            var orderItems = new List<OrderItem>();

            var order = new Order
            {
                OrderCode = GenerateOrderCode(now, "ORR"),
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
                SubTotal = 0,
                DiscountAmount = null,
                TaxAmount = null,
                DeliveryPrice = null,
                TotalAmount = 0,
                CreatedAt = DateTime.Now,
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

            var now = DateTime.Now;
            var tableOrders = await ValidateAndBuildTableOrders(request.TableIds);
            var subTotal = 0m;
            var orderItems = new List<OrderItem>();


            var order = new Order
            {
                OrderCode = GenerateOrderCode(now, "ORC"),
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
                SubTotal = 0,
                DiscountAmount = null,
                TaxAmount = null,
                DeliveryPrice = null,
                TotalAmount = 0,
                CreatedAt = DateTime.Now,
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

            var now = DateTime.Now;
            var tableOrders = await ValidateAndBuildTableOrders(request.TableIds);
            var subTotal = 0m;
            var orderItems = new List<OrderItem>();


            var order = new Order
            {
                OrderCode = GenerateOrderCode(now, "ORG"),
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
                SubTotal = 0,
                DiscountAmount = null,
                TaxAmount = null,
                DeliveryPrice = null,
                TotalAmount = 0,
                CreatedAt = DateTime.Now,
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

            var now = DateTime.Now;
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

        private static string GenerateOrderCode(DateTime now, string code)
        {
            var random = Random.Shared.Next(1000, 10000);
            return $"{code}-{now:yyyyMMddHHmmss}-{random}";
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

        public async Task<(bool status, string message)> ChooseAssignedStaffbyOrderAsync(ChooseAssignedStaffRequestDTO request)
        {
            var result = await _orderRepository.ChooseAssignedStaffbyOrderAsync(request);

            if (result.status)
            {
                var order = await _orderRepository.GetOrderByIdNoTrackingAsync(request.OrderCode);
                if (order != null)
                {
                    await _notificationService.CreateAutoNotificationAsync(
                        userId: order.UserId,
                        senderId: null,
                        title: "Đơn hàng đã được xác nhận",
                        content: $"Đơn hàng {request.OrderCode} đã được xác nhận " +
                                 $"và gán nhân viên giao hàng. Đơn hàng sẽ sớm được giao đến bạn!",
                        type: "Order",
                        severity: "Information"
                    );
                }
            }

            return result;
        }

        public async Task<(bool status, string message)> ChangeStatusDeliveryAsync(string request)
        {
            var result = await _orderRepository.ChangeStatusDeliveryAsync(request);

            if (result.status)
            {
                // Lấy order để có UserId của khách
                var order = await _orderRepository.GetOrderByIdNoTrackingAsync(request);
                if (order != null)
                {
                    // Xác định nội dung theo trạng thái mới
                    var deliveryStatus = order.Delivery?.DeliveryStatus;
                    string? title = null;
                    string? content = null;
                    string severity = "Information";

                    switch (deliveryStatus)
                    {
                        case "PickingUp":
                            title = "Đơn hàng đang được lấy";
                            content = $"Đơn hàng {request} đang được nhân viên lấy hàng chuẩn bị giao.";
                            break;
                        case "Delivering":
                            title = "Đơn hàng đang giao";
                            content = $"Đơn hàng {request} đang trên đường giao đến bạn.";
                            break;
                        case "Completed":
                            title = "Giao hàng thành công";
                            content = $"Đơn hàng {request} đã được giao thành công. Cảm ơn bạn!";
                            break;
                    }

                    if (title != null)
                    {
                        await _notificationService.CreateAutoNotificationAsync(
                            userId: order.UserId,
                            senderId: null,
                            title: title,
                            content: content!,
                            type: "Order",
                            severity: severity
                        );
                    }
                }
            }

            return result;
        }

        public async Task<(bool status, string message)> DeleteOrderDeliveryByDeliveryCodeAsync(string request, string dto)
        {
            var order = await _orderRepository.GetOrderByIdNoTrackingAsync(request);

            var result = await _orderRepository.DeleteOrderDeliveryByDeliveryCodeAsync(request, dto);

            if (result.status && order != null)
            {
                await _notificationService.CreateAutoNotificationAsync(
                    userId: order.UserId,
                    senderId: null,
                    title: "Đơn hàng đã bị hủy",
                    content: $"Đơn hàng {request} đã bị hủy. Lý do: {dto}",
                    type: "Order",
                    severity: "Warning"
                );
            }

            return result;
        }
        // ─── SESSION MENU ────────────────────────────────────────────────────────
        /// Lấy menu cho khách order trong session bàn (sau khi quét QR).
        /// Validate access token trước khi trả dữ liệu.

        public async Task<(bool Success, string? ErrorCode, object? Data)> GetMenuForSessionAsync(
     string accessToken, string? type, int? categoryId, string? keyword)
        {
            if (string.IsNullOrWhiteSpace(accessToken))
                return (false, "MISSING_TABLE_TOKEN", null);

            // Reuse logic validate token sẵn có trong TableService
            var validateResult = _tableService.ValidateAccessToken(accessToken);
            if (!validateResult.Valid)
                return (false, validateResult.ErrorCode, null);

            var data = await _orderRepository.GetMenuForSessionAsync(type, categoryId, keyword);
            return (true, null, data);
        }
        /// Lấy order hiện tại của bàn trong session (giỏ hàng khách đang xem).
        /// Reuse GetOrderDetailByOrderCodeAsync đã có.
        public async Task<(bool Success, string? ErrorCode, object? Data)> GetCurrentOrderBySessionAsync(string accessToken)
        {
            if (string.IsNullOrWhiteSpace(accessToken))
                return (false, "MISSING_TABLE_TOKEN", null);

            var validateResult = _tableService.ValidateAccessToken(accessToken);
            if (!validateResult.Valid)
                return (false, validateResult.ErrorCode, null);

            if (!int.TryParse(validateResult.TableCode, out int tableId))
                return (false, "INVALID_QR_TOKEN", null);

            // Reuse TableDAO.GetActiveOrderCodeByTableIdAsync qua repository
            var orderCode = await _orderRepository.GetActiveOrderCodeByTableIdAsync(tableId);
            if (string.IsNullOrEmpty(orderCode))
                return (false, "NO_ACTIVE_ORDER", null);

            // Reuse GetOrderDetailByOrderCodeAsync đã có
            var order = await _orderRepository.GetOrderDetailByOrderCodeAsync(orderCode);
            return (true, null, order);
        }

        public async Task<(bool status, string message)> AddDiscountToOrderAsync(string ordercode, string discountcode)
        {
            return await _orderRepository.AddDiscountToOrderAsync( ordercode,  discountcode);
        }
    }
}