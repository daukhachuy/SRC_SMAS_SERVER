using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SMAS_BusinessObject.Configurations;
using SMAS_BusinessObject.Models;

namespace SMAS_API.BackgroundJobs;

/// <summary>
/// Định kỳ hủy đơn Delivery ở trạng thái Pending mà chưa có thanh toán sau khoảng thời gian cấu hình.
/// </summary>
public class AutoCancelExpiredDeliveryOrdersJob : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IOptionsMonitor<AppSettings> _appSettings;
    private readonly ILogger<AutoCancelExpiredDeliveryOrdersJob> _logger;

    public AutoCancelExpiredDeliveryOrdersJob(
        IServiceScopeFactory scopeFactory,
        IOptionsMonitor<AppSettings> appSettings,
        ILogger<AutoCancelExpiredDeliveryOrdersJob> logger)
    {
        _scopeFactory = scopeFactory;
        _appSettings = appSettings;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var settings = _appSettings.CurrentValue;
            var sweepInterval = Math.Max(1, settings.DeliveryOrderSweepIntervalMinutes);
            var expireMinutes = Math.Max(1, settings.DeliveryOrderExpireMinutes);

            try
            {
                await using var scope = _scopeFactory.CreateAsyncScope();
                var context = scope.ServiceProvider.GetRequiredService<RestaurantDbContext>();

                var cutoff = DateTime.Now.AddMinutes(-expireMinutes);

                var expiredOrders = await context.Orders
                    .Include(o => o.Payments)
                    .Include(o => o.Delivery)
                    .Where(o => o.OrderType == "Delivery"
                             && o.OrderStatus == "Pending"
                             && o.CreatedAt < cutoff
                             && !o.Payments.Any(p => p.PaymentStatus == "Paid"))
                    .ToListAsync(stoppingToken);

                if (expiredOrders.Count > 0)
                {
                    foreach (var order in expiredOrders)
                    {
                        order.OrderStatus = "Cancelled";
                        if (order.Delivery != null)
                        {
                            order.Delivery.DeliveryStatus = "Failed";
                            order.Delivery.UpdatedAt = DateTime.Now;
                        }
                    }

                    await context.SaveChangesAsync(stoppingToken);
                    _logger.LogInformation(
                        "Auto-cancel: Đã hủy {Count} đơn Delivery quá {Minutes} phút chưa thanh toán.",
                        expiredOrders.Count, expireMinutes);
                }
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Lỗi khi chạy job auto-cancel đơn Delivery quá hạn.");
            }

            try
            {
                await Task.Delay(TimeSpan.FromMinutes(sweepInterval), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }
}
