namespace SMAS_BusinessObject.Configurations;

/// <summary>Cấu hình URL công khai (link ký, redirect frontend).</summary>
public class AppSettings
{
    public const string SectionName = "App";

    /// <summary>Base URL API công khai (vd: https://api.example.com) — dùng build link ký hợp đồng.</summary>
    public string PublicBaseUrl { get; set; } = "https://localhost:7001";

    /// <summary>URL frontend — redirect sau PayOS callback.</summary>
    public string FrontendBaseUrl { get; set; } = "http://localhost:3000";

    /// <summary>Số giờ kể từ thời điểm ký (SignedAt) để khách phải hoàn tất cọc (UTC).</summary>
    public int DepositDeadlineHoursAfterSign { get; set; } = 24;

    /// <summary>Chu kỳ chạy job hủy hợp đồng Signed quá hạn cọc (phút).</summary>
    public int ContractExpirationSweepIntervalMinutes { get; set; } = 5;

    /// <summary>Số phút đơn Delivery ở trạng thái Pending mà chưa thanh toán sẽ bị tự động hủy.</summary>
    public int DeliveryOrderExpireMinutes { get; set; } = 30;

    /// <summary>Chu kỳ chạy job hủy đơn Delivery quá hạn (phút).</summary>
    public int DeliveryOrderSweepIntervalMinutes { get; set; } = 5;

    /// <summary>Số giờ trước thời điểm diễn ra sự kiện để gửi nhắc manager.</summary>
    public int EventReminderHoursBeforeStart { get; set; } = 3;

    /// <summary>Chu kỳ chạy job quét nhắc sự kiện sắp diễn ra (phút).</summary>
    public int EventReminderSweepIntervalMinutes { get; set; } = 10;
    /// <summary>Số tiền phạt mỗi phút đi trễ (VNĐ).</summary>
    public decimal PenaltyPerLateMinute { get; set; } = 2000;

    /// <summary>Thưởng cố định khi đủ công cả tháng (VNĐ).</summary>
    public decimal FullMonthBonusAmount { get; set; } = 500000;

    /// <summary>Lương theo giờ mặc định dùng tính overtime (VNĐ).</summary>
    public decimal DefaultSalaryPerHour { get; set; } = 30000;

    /// <summary>Chu kỳ chạy job tính lương hàng tháng (giờ).</summary>
    public int SalarySweepIntervalHours { get; set; } = 6;
}
