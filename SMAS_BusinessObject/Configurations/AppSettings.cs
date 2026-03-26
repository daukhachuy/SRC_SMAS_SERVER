namespace SMAS_BusinessObject.Configurations;

/// <summary>Cấu hình URL công khai (link ký, redirect frontend).</summary>
public class AppSettings
{
    public const string SectionName = "App";

    /// <summary>Base URL API công khai (vd: https://api.example.com) — dùng build link ký hợp đồng.</summary>
    public string PublicBaseUrl { get; set; } = "https://localhost:7001";

    /// <summary>URL frontend — redirect sau PayOS callback.</summary>
    public string FrontendBaseUrl { get; set; } = "http://localhost:3000";
}
