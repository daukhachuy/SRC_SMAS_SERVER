using Microsoft.Extensions.Options;
using SMAS_BusinessObject.Configurations;
using SMAS_Services.ContractWorkflow;

namespace SMAS_API.BackgroundJobs;

/// <summary>
/// Định kỳ hủy hợp đồng Signed quá hạn cọc (theo SignedAt + App:DepositDeadlineHoursAfterSign).
/// </summary>
public class ContractDepositExpirationHostedService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IOptionsMonitor<AppSettings> _appSettings;
    private readonly ILogger<ContractDepositExpirationHostedService> _logger;

    public ContractDepositExpirationHostedService(
        IServiceScopeFactory scopeFactory,
        IOptionsMonitor<AppSettings> appSettings,
        ILogger<ContractDepositExpirationHostedService> logger)
    {
        _scopeFactory = scopeFactory;
        _appSettings = appSettings;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var minutes = Math.Max(1, _appSettings.CurrentValue.ContractExpirationSweepIntervalMinutes);
            try
            {
                await using var scope = _scopeFactory.CreateAsyncScope();
                var wf = scope.ServiceProvider.GetRequiredService<IContractWorkflowService>();
                var n = await wf.CancelExpiredSignedDepositContractsAsync();
                if (n > 0)
                    _logger.LogInformation("Đã hủy {Count} hợp đồng Signed quá hạn cọc.", n);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi chạy job hủy hợp đồng quá hạn cọc.");
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
