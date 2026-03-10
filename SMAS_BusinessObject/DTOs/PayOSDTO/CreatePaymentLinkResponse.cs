namespace SMAS_BusinessObject.DTOs.PayOSDTO;

/// <summary>
/// Response trả về cho Frontend sau khi tạo payment link PayOS thành công.
/// </summary>
public class CreatePaymentLinkResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    /// <summary>
    /// URL redirect khách hàng đến trang thanh toán PayOS.
    /// </summary>
    public string? CheckoutUrl { get; set; }
    public string? QrCode { get; set; }
    public string? PaymentLinkId { get; set; }
}
