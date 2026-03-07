using System.Text.Json.Serialization;

namespace SMAS_BusinessObject.DTOs.PayOSDTO;

/// <summary>
/// Payload mà PayOS gửi đến Webhook khi có kết quả thanh toán.
/// Ref: https://payos.vn/docs/tich-hop-webhook/
/// </summary>
public class PayOSWebhookPayload
{
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    [JsonPropertyName("desc")]
    public string? Desc { get; set; }

    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("data")]
    public PayOSWebhookData? Data { get; set; }

    [JsonPropertyName("signature")]
    public string? Signature { get; set; }
}

public class PayOSWebhookData
{
    [JsonPropertyName("orderCode")]
    public long OrderCode { get; set; }

    [JsonPropertyName("amount")]
    public long Amount { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("accountNumber")]
    public string? AccountNumber { get; set; }

    [JsonPropertyName("reference")]
    public string? Reference { get; set; }

    [JsonPropertyName("transactionDateTime")]
    public string? TransactionDateTime { get; set; }

    [JsonPropertyName("currency")]
    public string? Currency { get; set; }

    [JsonPropertyName("paymentLinkId")]
    public string? PaymentLinkId { get; set; }
}
