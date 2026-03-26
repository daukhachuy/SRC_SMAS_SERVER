using SMAS_BusinessObject.DTOs.PayOSDTO;

namespace SMAS_Services.PaymentServices;

public interface IPaymentService
{
    /// <summary>
    /// Tạo link thanh toán PayOS cho đơn hàng (order phải ở trạng thái Pending và thuộc user).
    /// </summary>
    Task<CreatePaymentLinkResponse> CreatePaymentLinkAsync(CreatePaymentLinkRequest request, int userId);

    /// <summary>
    /// Xử lý webhook từ PayOS khi có kết quả thanh toán; verify signature và cập nhật Order status.
    /// </summary>
    /// <param name="rawBody">Raw JSON body từ request (để build đúng chuỗi ký).</param>
    Task<bool> HandleWebhookAsync(string rawBody);

    /// <summary>PayOS — tạo link thanh toán tiền cọc hợp đồng (không qua Order).</summary>
    Task<ContractDepositPayOSResult> CreateContractDepositPaymentLinkAsync(
        long orderCode,
        int amountVnd,
        string description,
        string returnUrl,
        string cancelUrl);

    /// <summary>GET /v2/payment-requests/{orderCode} — xác minh đã thanh toán (PAID).</summary>
    Task<bool> VerifyPaymentAsync(long orderCode);

    /// <summary>Verify chữ ký webhook; ưu tiên header x-payos-signature, sau đó trường signature trong JSON.</summary>
    bool VerifyWebhookSignature(string rawBody, string? signatureHeader);
}
