using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using SMAS_BusinessObject.DTOs.PayOSDTO;
using SMAS_BusinessObject.Models;
using SMAS_Repositories.OrderRepositories;

namespace SMAS_Services.PaymentServices;

public class PaymentService : IPaymentService
{
    private readonly IOrderRepository _orderRepository;
    private readonly PayOSSettings _payOsSettings;
    private static readonly HttpClient SharedHttpClient = new() { BaseAddress = new Uri("https://api-merchant.payos.vn") };

    public PaymentService(
        IOrderRepository orderRepository,
        IOptions<PayOSSettings> payOsSettings)
    {
        _orderRepository = orderRepository;
        _payOsSettings = payOsSettings.Value;
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
            return true;

        int orderId = (int)payload.Data.OrderCode;
        var data = payload.Data;

        var payment = new Payment
        {
            PaymentCode = "PAY" + Guid.NewGuid().ToString("N")[..12].ToUpperInvariant(),
            OrderId = orderId,
            PaymentMethod = "PayOS",
            PaymentStatus = "Completed",
            Amount = data.Amount,
            TransactionId = data.Reference ?? data.PaymentLinkId,
            PaidAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            Note = data.Description
        };

        await _orderRepository.AddPaymentAndUpdateOrderStatusAsync(orderId, "Paid", payment);
        return true;
    }

    public async Task<ContractDepositPayOSResult> CreateContractDepositPaymentLinkAsync(
        long orderCode,
        int amountVnd,
        string description,
        string returnUrl,
        string cancelUrl)
    {
        if (amountVnd <= 0)
            return new ContractDepositPayOSResult { Success = false, Message = "Số tiền không hợp lệ." };

        string desc = description.Length > 25 ? description[..25] : description;

        string dataStr = $"amount={amountVnd}&cancelUrl={cancelUrl}&description={desc}&orderCode={orderCode}&returnUrl={returnUrl}";
        string signature = ComputeHmacSha256(dataStr, _payOsSettings.ChecksumKey);

        var body = new
        {
            orderCode,
            amount = (long)amountVnd,
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
        if (root.TryGetProperty("data", out var data) && data.ValueKind == JsonValueKind.Object)
        {
            string? checkoutUrl = data.TryGetProperty("checkoutUrl", out var url) ? url.GetString() : null;
            return new ContractDepositPayOSResult
            {
                Success = true,
                CheckoutUrl = checkoutUrl,
                Message = checkoutUrl == null ? "PayOS không trả về link thanh toán." : null
            };
        }

        string? errMsg = root.TryGetProperty("desc", out var descEl) ? descEl.GetString() : null;
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
}
