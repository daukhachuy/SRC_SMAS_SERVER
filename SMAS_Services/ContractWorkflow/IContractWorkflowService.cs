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

    Task<(ContractDetailByTokenDTO? dto, int statusCode, string? error)> GetContractByTokenAsync(string? token);

    Task<(ContractSignResponseDTO? dto, int statusCode, string? error)> SignContractByTokenAsync(ContractSignRequestDTO request);

    Task<(object? dto, int statusCode, string? error)> DepositAsync(int contractId, string apiBaseUrl);

    Task<string> DepositCallbackRedirectAsync(
        int contractId, string status, long orderCode, string frontendBaseUrl);

    Task DepositWebhookAsync(int contractId, string rawBody, string? signatureHeader);

    Task<(ConfirmBookEventResponseDTO? dto, int statusCode, string? error)> ConfirmBookEventAsync(
        int bookEventId, int staffUserId);
}
