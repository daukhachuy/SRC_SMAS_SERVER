using SMAS_BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;


namespace SMAS_API.BackgroundJobs
{
    public class OrderDiningTimeJob : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<OrderDiningTimeJob> _logger;

        public OrderDiningTimeJob(
            IServiceScopeFactory scopeFactory,
            ILogger<OrderDiningTimeJob> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await using var scope = _scopeFactory.CreateAsyncScope();
                    var context = scope.ServiceProvider.GetRequiredService<RestaurantDbContext>();

                    var now = DateTime.Now;

                    var orders = await context.Orders
                        .Include(o => o.OrderItems)
                        .ToListAsync(stoppingToken);

                    foreach (var order in orders)
                    {
                        if (order.CreatedAt == null) continue;

                        var hasBuffet = order.OrderItems.Any(i => i.BuffetId != null);
                        var minutes = (now - order.CreatedAt).TotalMinutes;
                        
                        if (hasBuffet)
                        {
                            var orderBuffettime = order.OrderItems.FirstOrDefault(i => i.BuffetId != null)?.OpeningTime;
                            if (orderBuffettime == null) continue;                          
                             minutes = (now - orderBuffettime.Value).TotalMinutes;
                            if (91 == minutes &&  minutes == 90)
                            {
                                await SendNotification(context, order,
                                    "Sắp hết giờ buffet",
                                    "Đơn đã dùng buffet 1h30p, còn 30 phút trước khi đóng gọi món.",
                                    "Warning");
                            }

                            if (121 == minutes && minutes == 120)
                            {

                                await SendNotification(context, order,
                                    "Đã đóng gọi món",
                                    "Đơn buffet đã hết thời gian 2 tiếng, hệ thống đã đóng gọi món.",
                                    "Error");

                            }
                        }

                        else
                        {
                            if (181 == minutes && minutes == 180)
                            {
                                await SendNotification(context, order,
                                    "Đơn đã ăn lâu",
                                    "Đơn đã dùng 3 tiếng, sẽ đóng gọi món sau 30 phút.",
                                    "Warning");

                            }

                            if (211 == minutes && minutes == 210)
                            {
                                await SendNotification(context, order,
                                    "Đã đóng gọi món",
                                    "Đơn đã quá 3 tiếng 30 phút, hãy đống gọi món cho đơn này .",
                                    "Error");

                            }
                        }
                    }

                    await context.SaveChangesAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi OrderDiningTimeJob");
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        private async Task SendNotification(RestaurantDbContext context, Order order,
            string title, string content, string severity)
        {
                context.Notifications.Add(new Notification
                {
                    UserId = order.ServedBy ?? 0,
                    Title = title,
                    Content = $"Order #{order.OrderId}: {content}",
                    Type = "Order",
                    Severity = severity,
                    IsRead = false,
                    CreatedAt = DateTime.Now
                });
            
        }
    }
}