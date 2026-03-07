namespace SMAS_BusinessObject.DTOs.PayOSDTO;

/// <summary>
/// Cấu hình PayOS (bind từ appsettings PayOS).
/// </summary>
public class PayOSSettings
{
    public const string SectionName = "PayOS";

    public string ClientId { get; set; } = null!;
    public string ApiKey { get; set; } = null!;
    public string ChecksumKey { get; set; } = null!;
    /// <summary>
    /// Base URL frontend (để build returnUrl/cancelUrl nếu client không gửi).
    /// </summary>
    public string? FrontendBaseUrl { get; set; }
}
