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
}
