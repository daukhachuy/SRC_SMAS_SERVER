using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SMAS_BusinessObject.DTOs.OrderDTO;
using SMAS_BusinessObject.Models;
using SMAS_DataAccess.DAO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Repositories.OrderRepositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrderDAO _orderDAO;
        private readonly ComboDAO _comboDAO;
        private readonly FoodDAO _foodDAO;
        private readonly DiscountDao _discountDAO;
        private readonly RestaurantDbContext _context;
        public OrderRepository(OrderDAO orderDAO,ComboDAO comboDAO, FoodDAO foodDAO, DiscountDao discountDAO, RestaurantDbContext context)
        {
            _orderDAO = orderDAO;
            _comboDAO = comboDAO;
            _foodDAO = foodDAO;
            _discountDAO = discountDAO;
            _context = context;
        }

        public async Task<IEnumerable<OrderListResponseDTO>> GetOrdersByUserAndStatusAsync(OrderListStatusRequest request, int userid)
        {
            var orders = await _orderDAO.GetOrdersByUserAndStatusAsync(userid, request.orderType, request.status);

            return orders.Select(o => new OrderListResponseDTO
            {
                OrderId = o.OrderId,
                OrderCode = o.OrderCode,
                OrderType = o.OrderType,
                OrderStatus = o.OrderStatus,
                NumberOfGuests = o.NumberOfGuests,
                SubTotal = o.SubTotal,
                DiscountAmount = o.DiscountAmount,
                TaxAmount = o.TaxAmount,
                DeliveryPrice = o.DeliveryPrice,
                TotalAmount = o.TotalAmount,
                Note = o.Note,
                CreatedAt = o.CreatedAt,
                ClosedAt = o.ClosedAt,

                Customer = new UserInfoDto
                {
                    UserId = o.User.UserId,
                    Fullname = o.User.Fullname,
                    Phone = o.User.Phone,
                    Email = o.User.Email
                },

                ServedBy = o.ServedByNavigation == null ? null : new StaffInfoDto
                {
                    UserId = o.ServedByNavigation.UserId,
                    Fullname = o.ServedByNavigation.User.Fullname
                },

                Delivery = o.Delivery == null ? null : new DeliveryDto
                {
                    DeliveryId = o.Delivery.DeliveryId,
                    RecipientName = o.Delivery.RecipientName,
                    RecipientPhone = o.Delivery.RecipientPhone,
                    Address = o.Delivery.Address,
                    DeliveryStatus = o.Delivery.DeliveryStatus,
                    DeliveryFee = o.Delivery.DeliveryFee,
                    EstimatedDeliveryTime = o.Delivery.EstimatedDeliveryTime,
                    ActualDeliveryTime = o.Delivery.ActualDeliveryTime
                },

                Payments = o.Payments.Select(p => new PaymentDto
                {
                    PaymentId = p.PaymentId,
                    PaymentMethod = p.PaymentMethod,
                    PaymentStatus = p.PaymentStatus,
                    Amount = p.Amount,
                    PaidAt = p.PaidAt
                }).ToList(),

                Items = o.OrderItems.Select(i => new OrderItemDetailDto
                {
                    OrderItemId = i.OrderItemId,
                    ItemType = i.Food != null ? "Food"
                              : i.Combo != null ? "Combo"
                              : "Buffet",

                    ItemName = i.Food?.Name
                            ?? i.Combo?.Name
                            ?? i.Buffet?.Name
                            ?? "",

                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    Subtotal = i.Subtotal,
                    Status = i.Status,
                    OpeningTime = i.OpeningTime,
                    ServedTime = i.ServedTime
                }).ToList()
            }).ToList();
        }

        public async Task<OrderDeliveryResponse> CreateOrderDeliveryAsync(CreateOrderDeliveryRequest request, int userid)
        {

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (request.Items == null || !request.Items.Any())
                    return new OrderDeliveryResponse { Success = false, Message = "Giỏ hàng trống." };

                var order = new Order
                {
                    UserId = userid,
                    OrderCode = "ORD-" + DateTime.Now.Ticks.ToString().Substring(10),
                    OrderType =  "Delivery",
                    Note = request.Note,
                    OrderStatus = "Pending",
                    CreatedAt = DateTime.Now
                };

                decimal subTotal = 0;

                foreach (var item in request.Items)
                {
                    decimal unitPrice = 0;
                    string itemType = "";
                    if (item.FoodId.HasValue)
                    {
                        unitPrice = await _foodDAO.GetFoodPriceAsync(item.FoodId.Value);
                        itemType = "Food";
                        if (unitPrice == 0) return new OrderDeliveryResponse { Success = false, Message = $"Món ăn ID {item.FoodId} không tồn tại hoặc đã ngừng bán." };
                    }
                    else if (item.ComboId.HasValue)
                    {
                        unitPrice = await _comboDAO.GetComboPriceAsync(item.ComboId.Value);
                        itemType = "Combo";
                        if (unitPrice == 0) return new OrderDeliveryResponse { Success = false, Message = $"Combo ID {item.ComboId} không tồn tại hoặc đã ngừng bán." };
                    }           

                    var orderItem = new OrderItem
                    {
                        FoodId = item.FoodId,
                        ComboId = item.ComboId,
                        Quantity = item.Quantity,
                        UnitPrice = unitPrice,
                        Status = "Pending",
                        OpeningTime = DateTime.Now
                    };

                    order.OrderItems.Add(orderItem);
                    subTotal += (unitPrice * item.Quantity);
                }

                order.SubTotal = subTotal;

                // --- Logic Giảm giá (Discount) ---
                if (!string.IsNullOrEmpty(request.DiscountCode))
                {
                    var discount = await _discountDAO.GetDiscountByIdAsync(request.DiscountCode);
                    if (discount == null)
                        return new OrderDeliveryResponse { Success = false, Message = "Mã giảm giá không hợp lệ." };

                    if (subTotal < discount.MinOrderAmount)
                        return new OrderDeliveryResponse { Success = false, Message = $"Đơn hàng chưa đạt giá trị tối thiểu {discount.MinOrderAmount} để áp dụng mã." };

                    order.DiscountId = discount.DiscountId;
                    decimal amount = (discount.DiscountType == "Percentage") ? (subTotal * (discount.Value / 100)) : discount.Value;
                    order.DiscountAmount = Math.Min(amount, discount.MaxDiscountAmount ?? amount);
                }

                // --- Logic Giao hàng (Delivery) ---
                if (order.OrderType == "Delivery")
                {
                    if (request.DeliveryInfo == null)
                        return new OrderDeliveryResponse { Success = false, Message = "Thiếu thông tin giao hàng." };
                    //double distance = CalculateDistance(
                    //            16.0256325, 108.2178437,
                    //            request.DeliveryInfo.Latitude,
                    //            request.DeliveryInfo.Longitude
                    //);

                    var location = await GetCoordinatesFromAddress(request.DeliveryInfo.Address);

                    double userLat = location.lat;
                    double userLng = location.lon;
                    if (userLat == 0 && userLng == 0)
                        return new OrderDeliveryResponse { Success = false, Message = "Không thể xác định tọa độ từ địa chỉ đã cung cấp." };

                    double distance = CalculateDistance(16.0256325, 108.2178437, userLat, userLng);


                    if (distance > 20)
                        return new OrderDeliveryResponse
                        {
                            Success = false,
                            Message = "Địa chỉ nằm ngoài phạm vi giao hàng (20km)."
                        };
                    order.Delivery = new DeliveryDetail
                    {
                        RecipientName = request.DeliveryInfo.RecipientName,
                        RecipientPhone = request.DeliveryInfo.RecipientPhone,
                        Address = request.DeliveryInfo.Address,
                        DeliveryStatus = "Pending",
                        DeliveryCode = "ORD-" + DateTime.Now.Ticks.ToString().Substring(10),
                        Note = request.DeliveryInfo.Note

                    };
                    if (distance <= 5)
                        order.DeliveryPrice = 15000;
                    else if (distance <= 10)
                        order.DeliveryPrice = 25000;
                    else
                        order.DeliveryPrice = 40000;
                }

                order.TotalAmount = (order.SubTotal ?? 0) - (order.DiscountAmount ?? 0) + (order.DeliveryPrice ?? 0);

                var result = await _orderDAO.CreateOrderDeliveryAsync(order);

                await transaction.CommitAsync();
                return new OrderDeliveryResponse { Success = true, Message = $"Đơn hàng của bạn đã thêm thành công." };
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return new OrderDeliveryResponse { Success = false, Message = "Lỗi hệ thống khi xử lý đơn hàng. Vui lòng thử lại sau." };
            }
        }


        public async Task<(double lat, double lon)> GetCoordinatesFromAddress(string address)
        {
            var fullAddress = address + ", Da Nang, Vietnam";
            var client = new HttpClient();

            client.DefaultRequestHeaders.Add("User-Agent", "RestaurantDeliveryApp");

            var url = $"https://nominatim.openstreetmap.org/search?q={Uri.EscapeDataString(address)}&format=json&limit=1";

            var response = await client.GetAsync(url);

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            var data = JsonConvert.DeserializeObject<List<NominatimResponse>>(json);

            if (data == null || !data.Any()) return (0, 0);

            var lat = double.Parse(data[0].lat.Replace(",", "."), CultureInfo.InvariantCulture);
            var lon = double.Parse(data[0].lon.Replace(",", "."), CultureInfo.InvariantCulture);

            return (lat, lon);
        }

        public class NominatimResponse
        {
            public string lat { get; set; }
            public string lon { get; set; }
        }

        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            var R = 6371; // km
            var dLat = (lat2 - lat1) * Math.PI / 180;
            var dLon = (lon2 - lon1) * Math.PI / 180;

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(lat1 * Math.PI / 180) *
                    Math.Cos(lat2 * Math.PI / 180) *
                    Math.Sin(dLon / 2) *
                    Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return R * c;
        }
    }
}
