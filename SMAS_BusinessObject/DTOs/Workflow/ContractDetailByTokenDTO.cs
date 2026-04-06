namespace SMAS_BusinessObject.DTOs.Workflow;

/// <summary>Dữ liệu hiển thị hợp đồng trước khi ký (GET sign?token=).</summary>
public class ContractDetailByTokenDTO
{
    public string? ContractCode { get; set; }

    /// <summary>Ngày sự kiện (yyyy-MM-dd).</summary>
    public string EventDate { get; set; } = null!;

    public int? NumberOfGuests { get; set; }

    public decimal TotalAmount { get; set; }

    public decimal? DepositAmount { get; set; }

    public string? TermsAndConditions { get; set; }

    public string? ContractFileUrl { get; set; }

    /// <summary>Token dùng cho bước POST ký.</summary>
    public string Token { get; set; } = null!;
}
