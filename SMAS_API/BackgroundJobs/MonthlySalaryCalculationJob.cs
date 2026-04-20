using Microsoft.Extensions.Options;
using SMAS_BusinessObject.Configurations;
using SMAS_Services.SalaryService;

namespace SMAS_API.BackgroundJobs;

/// <summary>
/// Định kỳ kiểm tra và tính lương hàng tháng cho tất cả nhân viên vào ngày 1.
/// Idempotent: nếu SalaryRecord cho tháng trước đã tồn tại thì bỏ qua.
/// </summary>
public class MonthlySalaryCalculationJob : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IOptionsMonitor<AppSettings> _appSettings;
    private readonly ILogger<MonthlySalaryCalculationJob> _logger;

    public MonthlySalaryCalculationJob(
        IServiceScopeFactory scopeFactory,
        IOptionsMonitor<AppSettings> appSettings,
        ILogger<MonthlySalaryCalculationJob> logger)
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
            var intervalHours = Math.Max(1, settings.SalarySweepIntervalHours);

            try
            {
                if (DateTime.Now.Day == 1)
                {
                    var previousMonth = DateTime.Now.AddMonths(-1);
                    var month = previousMonth.Month;
                    var year = previousMonth.Year;

                    await using var scope = _scopeFactory.CreateAsyncScope();
                    var salaryService = scope.ServiceProvider.GetRequiredService<ISalaryRecordService>();

                    var count = await salaryService.CalculateAndSaveMonthlySalaryAsync(
                        month, year,
                        settings.PenaltyPerLateMinute,
                        settings.FullMonthBonusAmount,
                        settings.DefaultSalaryPerHour);

                    if (count > 0)
                    {
                        _logger.LogInformation(
                            "MonthlySalary: Đã tính lương tháng {Month}/{Year} cho {Count} nhân viên.",
                            month, year, count);
                    }
                }
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Lỗi khi chạy job tính lương hàng tháng.");
            }

            try
            {
                await Task.Delay(TimeSpan.FromHours(intervalHours), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }
}
