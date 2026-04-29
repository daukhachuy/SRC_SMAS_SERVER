using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SMAS_BusinessObject.Models;

namespace SMAS_API.BackgroundJobs;

public class OrderItemTimeoutJob : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OrderItemTimeoutJob> _logger;

    public OrderItemTimeoutJob(
        IServiceScopeFactory scopeFactory,
        ILogger<OrderItemTimeoutJob> logger)
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
                var items = await context.OrderItems
                    .Where(x => x.Status == "Preparing" && x.OpeningTime != null)
                    .ToListAsync(stoppingToken);
                var users = await context.Users.Include(u => u.Staff)
                    .Where(u => u.Staff.Position == "Kitchen" && u.Staff.IsWorking == true)
                    .ToListAsync(stoppingToken);
                foreach (var item in items)
                {
                    var minutes = (now - item.OpeningTime.Value).TotalMinutes;

                    if (minutes == 15 || minutes == 16)
                    {
                        _logger.LogWarning($"[NOTIFY] Món {item.OrderItemId} đã chờ {minutes} phút!");
                        foreach (var user in users)
                        {
                            context.Notifications.Add(new Notification
                            {
                                UserId = user.UserId,
                                Title = "Món ăn bị chậm",
                                Content = $"Món #{item.OrderItemId} đã chờ {Math.Floor(minutes)} phút.",
                                Type = "Order",
                                Severity = "Warning",
                                IsRead = false,
                                CreatedAt = DateTime.Now
                            });
                        }

                    }
                    if (minutes >= 30)
                    {
                        item.Status = "Cancelled";
                        item.Note = "Hệ thống tự hủy do quá thời gian chế biến";

                        _logger.LogWarning($"[AUTO CANCEL] Món {item.OrderItemId} bị hủy do quá giờ!");
                        foreach (var user in users)
                        {
                            context.Notifications.Add(new Notification
                            {
                                UserId = user.UserId,
                                Title = "Món ăn bị hủy",
                                Content = $"Món #{item.OrderItemId} đã bị hủy do quá thời gian chờ.",
                                Type = "Order",
                                Severity = "Error",
                                IsRead = false,
                                CreatedAt = DateTime.Now
                            });
                        }
                    }
                }

                await context.SaveChangesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi chạy OrderItemTimeoutJob");
            }
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}