using Microsoft.Extensions.Options;
using SMAS_BusinessObject.Configurations;
using SMAS_Services.BookEventService;

namespace SMAS_API.BackgroundJobs;

/// <summary>
/// Định kỳ gửi nhắc manager khi sự kiện Active còn <= N giờ sẽ diễn ra.
/// </summary>
public class UpcomingBookEventReminderHostedService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IOptionsMonitor<AppSettings> _appSettings;
    private readonly ILogger<UpcomingBookEventReminderHostedService> _logger;

    public UpcomingBookEventReminderHostedService(
        IServiceScopeFactory scopeFactory,
        IOptionsMonitor<AppSettings> appSettings,
        ILogger<UpcomingBookEventReminderHostedService> logger)
    {
        _scopeFactory = scopeFactory;
        _appSettings = appSettings;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var minutes = Math.Max(1, _appSettings.CurrentValue.EventReminderSweepIntervalMinutes);
            var hoursBeforeStart = Math.Max(1, _appSettings.CurrentValue.EventReminderHoursBeforeStart);
            try
            {
                await using var scope = _scopeFactory.CreateAsyncScope();
                var service = scope.ServiceProvider.GetRequiredService<IBookEventService>();
                var created = await service.NotifyManagersBeforeUpcomingEventsAsync(hoursBeforeStart);
                if (created > 0)
                    _logger.LogInformation("Đã tạo {Count} thông báo nhắc sự kiện sắp diễn ra.", created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi chạy job nhắc sự kiện sắp diễn ra.");
            }

            try
            {
                await Task.Delay(TimeSpan.FromMinutes(minutes), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }
}
