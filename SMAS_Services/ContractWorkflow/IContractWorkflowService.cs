using SMAS_BusinessObject.DTOs.Workflow;

namespace SMAS_Services.ContractWorkflow;

public interface IContractWorkflowService
{
    Task<(BookEventReviewResponseDTO? dto, int statusCode, string? error)> ReviewBookEventAsync(
        int bookEventId, BookEventReviewRequestDTO request, int staffUserId);

    Task<(CreateContractFromBookEventResponseDTO? dto, int statusCode, string? error)> CreateContractFromBookEventAsync(
        int bookEventId, CreateContractFromBookEventRequestDTO request);

    Task<(BookEventDetailResponseDTO? dto, int statusCode, string? error)> GetBookEventDetailAsync(int bookEventId);

    Task<(SendContractSignResponseDTO? dto, int statusCode, string? error)> SendContractSignEmailAsync(int contractId, string publicBaseUrl);

    Task<(ContractDetailByTokenDTO? dto, int statusCode, string? error)> GetContractByTokenAsync(string? token, int currentUserId);

    Task<(ContractSignResponseDTO? dto, int statusCode, string? error)> SignContractByTokenAsync(ContractSignRequestDTO request, int currentUserId);

    Task<(object? dto, int statusCode, string? error)> DepositAsync(
        int contractId,
        string apiBaseUrl,
        int currentUserId,
        bool isPrivilegedRole);

    Task<string> DepositCallbackRedirectAsync(
        int contractId, string status, long orderCode, string payosCode, string frontendBaseUrl);

    Task DepositWebhookAsync(int contractId, string rawBody, string? signatureHeader);

    /// <summary>Job nền: hủy Signed quá hạn cọc (theo SignedAt + cấu hình giờ).</summary>
    Task<int> CancelExpiredSignedDepositContractsAsync();

    /// <summary>
    /// Manager xác nhận thu phần còn lại (tiền mặt) sau khi checkout sự kiện.
    /// Nhập ContractCode (mã hợp đồng trên chứng từ) thay vì ContractId.
    /// Tạo Payment cash, chuyển Contract -> PaidInFull và BookEvent -> Completed.
    /// </summary>
    Task<(ConfirmContractFinalPaymentResponseDTO? dto, int statusCode, string? error)>
        ConfirmFinalPaymentCashAsync(string contractCode, ConfirmContractFinalPaymentRequestDTO request, int managerUserId);

    /// <summary>Lịch sử giao dịch của 1 hợp đồng (deposit + remaining + other), kèm tổng đã/sẽ thu.</summary>
    Task<(ContractPaymentHistoryResponseDTO? dto, int statusCode, string? error)>
        GetContractPaymentsByIdAsync(int contractId);

    /// <summary>Lịch sử giao dịch theo ContractCode (để manager tra theo mã hợp đồng).</summary>
    Task<(ContractPaymentHistoryResponseDTO? dto, int statusCode, string? error)>
        GetContractPaymentsByCodeAsync(string contractCode);
}
