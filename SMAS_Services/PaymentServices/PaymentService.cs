using Microsoft.Extensions.Options;
using SMAS_BusinessObject.DTOs.PayOSDTO;
using SMAS_BusinessObject.Models;
using SMAS_Repositories.OrderRepositories;
using SMAS_Services.NotificationServices;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace SMAS_Services.PaymentServices;

public class PaymentService : IPaymentService
{
    private readonly IOrderRepository _orderRepository;
    private readonly PayOSSettings _payOsSettings;
    private static readonly HttpClient SharedHttpClient = new() { BaseAddress = new Uri("https://api-merchant.payos.vn") };
    private readonly IPaymentRepository _paymentRepo;
    private readonly INotificationService _notificationService;

    public PaymentService(
        IOrderRepository orderRepository,
        IOptions<PayOSSettings> payOsSettings,
        IPaymentRepository paymentRepo,
        INotificationService notificationService)
    {
        _orderRepository = orderRepository;
        _payOsSettings = payOsSettings.Value;
        _paymentRepo = paymentRepo;
        _notificationService = notificationService;
    }

    public async Task<CreatePaymentLinkResponse> CreatePaymentLinkAsync(CreatePaymentLinkRequest request, int userId)
    {
        var order = await _orderRepository.GetOrderByIdAsync(request.OrderId);
        if (order == null)
            return new CreatePaymentLinkResponse { Success = false, Message = "Đơn hàng không tồn tại." };

        if (order.UserId != userId)
            return new CreatePaymentLinkResponse { Success = false, Message = "Bạn không có quyền thanh toán đơn hàng này." };

        if (order.OrderStatus != "Pending")
            return new CreatePaymentLinkResponse { Success = false, Message = "Đơn hàng không ở trạng thái chờ thanh toán." };

        long amountVnd = (long)Math.Round(order.TotalAmount);
        if (amountVnd <= 0)
            return new CreatePaymentLinkResponse { Success = false, Message = "Tổng tiền đơn hàng không hợp lệ." };

        int orderCode = order.OrderId;
        string description = (order.OrderCode ?? $"ORD{order.OrderId}").Length > 9
            ? $"ORD{order.OrderId}"
            : (order.OrderCode ?? $"ORD{order.OrderId}");

        // Chuỗi ký theo PayOS: key=value nối bằng &, sort theo key, KHÔNG encode URL (raw)
        string dataStr = $"amount={amountVnd}&cancelUrl={request.CancelUrl}&description={description}&orderCode={orderCode}&returnUrl={request.ReturnUrl}";
        string signature = ComputeHmacSha256(dataStr, _payOsSettings.ChecksumKey);

        var body = new
        {
            orderCode,
            amount = amountVnd,
            description,
            cancelUrl = request.CancelUrl,
            returnUrl = request.ReturnUrl,
            signature
        };

        var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/v2/payment-requests");
        requestMessage.Headers.Add("x-client-id", _payOsSettings.ClientId);
        requestMessage.Headers.Add("x-api-key", _payOsSettings.ApiKey);
        requestMessage.Content = new StringContent(
            JsonSerializer.Serialize(body),
            Encoding.UTF8,
            "application/json");

        var response = await SharedHttpClient.SendAsync(requestMessage);

        if (!response.IsSuccessStatusCode)
        {
            var errBody = await response.Content.ReadAsStringAsync();
            return new CreatePaymentLinkResponse
            {
                Success = false,
                Message = $"PayOS trả lỗi: {response.StatusCode}. {errBody}"
            };
        }

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;
        if (root.TryGetProperty("data", out var data) && data.ValueKind == JsonValueKind.Object)
        {
            string? checkoutUrl = data.TryGetProperty("checkoutUrl", out var url) ? url.GetString() : null;
            string? qrCode = data.TryGetProperty("qrCode", out var qr) ? qr.GetString() : null;
            string? paymentLinkId = data.TryGetProperty("paymentLinkId", out var id) ? id.GetString() : null;
            return new CreatePaymentLinkResponse
            {
                Success = true,
                CheckoutUrl = checkoutUrl,
                QrCode = qrCode,
                PaymentLinkId = paymentLinkId
            };
        }

        // PayOS trả 200 nhưng data null hoặc không phải object (có thể do lỗi từ PayOS)
        string? errMsg = root.TryGetProperty("desc", out var desc) ? desc.GetString() : null;
        return new CreatePaymentLinkResponse
        {
            Success = false,
            Message = errMsg ?? "PayOS không trả về link thanh toán."
        };
    }

    public async Task<bool> HandleWebhookAsync(string rawBody)
    {
        if (string.IsNullOrWhiteSpace(rawBody)) return false;

        PayOSWebhookPayload? payload;
        try
        {
            payload = JsonSerializer.Deserialize<PayOSWebhookPayload>(rawBody);
        }
        catch
        {
            return false;
        }

        if (payload?.Data == null)
            return false;

        // Request confirm webhook từ PayOS (payload mẫu orderCode 123, amount 3000): luôn trả 200 để đăng ký URL thành công
        if (payload.Data.OrderCode == 123 && payload.Data.Amount == 3000)
            return true;

        if (!VerifyWebhookSignature(rawBody, null))
            return false;

        if (!payload.Success)
        {
            int cancelledOrderId = (int)payload.Data.OrderCode;
            await _orderRepository.UpdateOrderStatusAsync(cancelledOrderId, "Cancelled");
            return true;
        }

        int orderId = (int)payload.Data.OrderCode;
        var data = payload.Data;

        var payment = new Payment
        {
            PaymentCode = "PAY" + Guid.NewGuid().ToString("N")[..12].ToUpperInvariant(),
            OrderId = orderId,
            PaymentMethod = "PayOS",
            PaymentStatus = "Paid",
            Amount = data.Amount,
            TransactionId = data.Reference ?? data.PaymentLinkId,
            PaidAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            Note = data.Description
        };

        await _orderRepository.AddPaymentAndAutoCompleteAsync(orderId, payment);
        var order = await _orderRepository.GetOrderByIdAsync(orderId);
        if (order != null)
        {
            await _notificationService.CreateAutoNotificationAsync(
                userId: order.UserId,
                senderId: null,
                title: "Thanh toán thành công",
                content: $"Đơn hàng {order.OrderCode} đã được thanh toán " +
                         $"{data.Amount:N0}đ qua PayOS. Cảm ơn bạn!",
                type: "Order",
                severity: "Information"
            );
        }

        return true;
    }

    public async Task<ContractDepositPayOSResult> CreateContractDepositPaymentLinkAsync(
        int orderCode,
        int amountVnd,
        string description,
        string returnUrl,
        string cancelUrl)
    {
        if (amountVnd <= 0)
            return new ContractDepositPayOSResult { Success = false, Message = "Số tiền không hợp lệ." };

        string desc = description.Length > 25 ? description[..25] : description;

        // Cùng công thức chuỗi ký + kiểu amount long như CreatePaymentLinkAsync (đơn mang đi) — không đổi method đó.
        long amountLong = amountVnd;
        string dataStr = $"amount={amountLong}&cancelUrl={cancelUrl}&description={desc}&orderCode={orderCode}&returnUrl={returnUrl}";
        string signature = ComputeHmacSha256(dataStr, _payOsSettings.ChecksumKey);

        var body = new
        {
            orderCode,
            amount = amountLong,
            description = desc,
            cancelUrl,
            returnUrl,
            signature
        };

        var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/v2/payment-requests");
        requestMessage.Headers.Add("x-client-id", _payOsSettings.ClientId);
        requestMessage.Headers.Add("x-api-key", _payOsSettings.ApiKey);
        requestMessage.Content = new StringContent(
            JsonSerializer.Serialize(body),
            Encoding.UTF8,
            "application/json");

        var response = await SharedHttpClient.SendAsync(requestMessage);

        if (!response.IsSuccessStatusCode)
        {
            var errBody = await response.Content.ReadAsStringAsync();
            return new ContractDepositPayOSResult
            {
                Success = false,
                Message = $"PayOS trả lỗi: {response.StatusCode}. {errBody}"
            };
        }

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        var code = root.TryGetProperty("code", out var codeEl) ? codeEl.GetString() : null;
        if (!string.IsNullOrEmpty(code) && code != "00")
        {
            var failDesc = root.TryGetProperty("desc", out var failDescEl) ? failDescEl.GetString() : null;
            return new ContractDepositPayOSResult
            {
                Success = false,
                Message = $"PayOS code={code}. {failDesc ?? json}"
            };
        }

        if (root.TryGetProperty("data", out var data) && data.ValueKind == JsonValueKind.Object)
        {
            string? checkoutUrl = data.TryGetProperty("checkoutUrl", out var url) ? url.GetString() : null;
            if (string.IsNullOrEmpty(checkoutUrl))
            {
                string? innerDesc = data.TryGetProperty("desc", out var d0) ? d0.GetString() : null;
                return new ContractDepositPayOSResult
                {
                    Success = false,
                    Message = innerDesc ?? "PayOS không trả về checkoutUrl (data có nhưng thiếu link)."
                };
            }

            return new ContractDepositPayOSResult
            {
                Success = true,
                CheckoutUrl = checkoutUrl
            };
        }

        string? errMsg = root.TryGetProperty("desc", out var descEl) ? descEl.GetString() : null;
        if (string.IsNullOrEmpty(errMsg) && root.TryGetProperty("message", out var msgEl))
            errMsg = msgEl.GetString();
        return new ContractDepositPayOSResult
        {
            Success = false,
            Message = errMsg ?? "PayOS không trả về link thanh toán."
        };
    }

    public async Task<bool> VerifyPaymentAsync(long orderCode)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"/v2/payment-requests/{orderCode}");
        request.Headers.Add("x-client-id", _payOsSettings.ClientId);
        request.Headers.Add("x-api-key", _payOsSettings.ApiKey);

        var response = await SharedHttpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
            return false;

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;
        if (!root.TryGetProperty("code", out var codeEl) || codeEl.GetString() != "00")
            return false;

        if (!root.TryGetProperty("data", out var data) || data.ValueKind != JsonValueKind.Object)
            return false;

        if (data.TryGetProperty("status", out var statusEl))
        {
            var status = statusEl.GetString();
            return string.Equals(status, "PAID", StringComparison.OrdinalIgnoreCase);
        }

        return false;
    }

    public bool VerifyWebhookSignature(string rawBody, string? signatureHeader)
    {
        if (string.IsNullOrWhiteSpace(rawBody))
            return false;

        PayOSWebhookPayload? payload;
        try
        {
            payload = JsonSerializer.Deserialize<PayOSWebhookPayload>(rawBody);
        }
        catch
        {
            return false;
        }

        if (payload?.Data == null)
            return false;

        string? signature = signatureHeader;
        if (string.IsNullOrEmpty(signature))
            signature = payload.Signature;

        if (string.IsNullOrEmpty(signature))
            return false;

        using var doc = JsonDocument.Parse(rawBody);
        var root = doc.RootElement;
        if (!root.TryGetProperty("data", out var dataElm))
            return false;

        string dataStr = BuildSortedDataString(dataElm);
        string expectedSignature = ComputeHmacSha256(dataStr, _payOsSettings.ChecksumKey);
        return string.Equals(expectedSignature, signature, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Build chuỗi data từ object "data" của webhook (sort key alphabetically).
    /// PayOS payment-requests: key=value nối trực tiếp, KHÔNG encode URI (theo tài liệu PayOS).
    /// </summary>
    private static string BuildSortedDataString(JsonElement dataElm)
    {
        var list = new List<string>();
        foreach (var prop in dataElm.EnumerateObject().OrderBy(p => p.Name, StringComparer.Ordinal))
        {
            string value = prop.Value.ValueKind == JsonValueKind.String
                ? (prop.Value.GetString() ?? "")
                : prop.Value.GetRawText();
            if (value == null || value == "null" || value == "undefined")
                value = "";
            list.Add($"{prop.Name}={value}");
        }
        return string.Join("&", list);
    }

    private static string ComputeHmacSha256(string data, string key)
    {
        var keyBytes = Encoding.UTF8.GetBytes(key);
        var dataBytes = Encoding.UTF8.GetBytes(data);
        using var hmac = new HMACSHA256(keyBytes);
        var hash = hmac.ComputeHash(dataBytes);
        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    }


    public async Task<(bool status, string message)> CreatePaymentOrderCashAsync(PaymentOrderCashRequestDTO payment, int userid)
    {
        return await _paymentRepo.CreatePaymentOrderCashAsync(payment,userid);
    }

    public async Task<CreatePaymentLinkResponse> CreateRemainingPaymentLinkAsync(RemainingPaymentQrRequestDTO request)
    {
        var (success, message, remaining, orderId) = await _paymentRepo.CreateRemainingPaymentLinkAsync(
            request.OrderCode, request.ReturnUrl, request.CancelUrl);

        if (!success)
            return new CreatePaymentLinkResponse { Success = false, Message = message };

        long amountVnd = (long)Math.Round(remaining);
        if (amountVnd <= 0)
            return new CreatePaymentLinkResponse { Success = false, Message = "Số tiền thanh toán không hợp lệ." };

        int orderCode = orderId;
        string description = request.OrderCode.Length <= 9
            ? request.OrderCode
            : $"ORD{orderId}";

        await CancelPaymentLinkAsync(orderCode);

        string dataStr = $"amount={amountVnd}&cancelUrl={request.CancelUrl}&description={description}&orderCode={orderCode}&returnUrl={request.ReturnUrl}";
        string signature = ComputeHmacSha256(dataStr, _payOsSettings.ChecksumKey);

        var body = new
        {
            orderCode,
            amount = amountVnd,
            description,
            cancelUrl = request.CancelUrl,
            returnUrl = request.ReturnUrl,
            signature
        };

        var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/v2/payment-requests");
        requestMessage.Headers.Add("x-client-id", _payOsSettings.ClientId);
        requestMessage.Headers.Add("x-api-key", _payOsSettings.ApiKey);
        requestMessage.Content = new StringContent(
            JsonSerializer.Serialize(body),
            Encoding.UTF8,
            "application/json");

        var response = await SharedHttpClient.SendAsync(requestMessage);

        if (!response.IsSuccessStatusCode)
        {
            var errBody = await response.Content.ReadAsStringAsync();
            return new CreatePaymentLinkResponse
            {
                Success = false,
                Message = $"PayOS trả lỗi: {response.StatusCode}. {errBody}"
            };
        }

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;
        if (root.TryGetProperty("data", out var data) && data.ValueKind == JsonValueKind.Object)
        {
            string? checkoutUrl = data.TryGetProperty("checkoutUrl", out var url) ? url.GetString() : null;
            string? qrCode = data.TryGetProperty("qrCode", out var qr) ? qr.GetString() : null;
            string? paymentLinkId = data.TryGetProperty("paymentLinkId", out var id) ? id.GetString() : null;
            return new CreatePaymentLinkResponse
            {
                Success = true,
                CheckoutUrl = checkoutUrl,
                QrCode = qrCode,
                PaymentLinkId = paymentLinkId
            };
        }

        string? errMsg = root.TryGetProperty("desc", out var desc) ? desc.GetString() : null;
        return new CreatePaymentLinkResponse
        {
            Success = false,
            Message = errMsg ?? "PayOS không trả về link thanh toán."
        };
    }

    private async Task CancelPaymentLinkAsync(int orderCode)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Put, $"/v2/payment-requests/{orderCode}/cancel");
            request.Headers.Add("x-client-id", _payOsSettings.ClientId);
            request.Headers.Add("x-api-key", _payOsSettings.ApiKey);
            request.Content = new StringContent(
                JsonSerializer.Serialize(new { cancellationReason = "Tạo lại link thanh toán còn lại" }),
                Encoding.UTF8,
                "application/json");
            await SharedHttpClient.SendAsync(request);
        }
        catch
        {
            // Bỏ qua lỗi cancel — link cũ có thể không tồn tại hoặc đã hết hạn
        }
    }

    public async Task<PagedResult<TransactionHistoryItemDTO>> GetTransactionHistoryAsync(
       TransactionHistoryRequestDTO request)
    {
        return await _paymentRepo.GetTransactionHistoryAsync(request);
    }

}
