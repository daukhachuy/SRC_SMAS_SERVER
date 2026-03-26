using SMAS_BusinessObject.Models;

namespace SMAS_Repositories.ContractWorkflow;

public interface IContractWorkflowRepository
{
    Task<BookEvent?> GetBookEventForReviewAsync(int bookEventId);
    Task<BookEvent?> GetBookEventForCreateContractAsync(int bookEventId);
    Task<BookEvent?> GetBookEventForDetailAsync(int bookEventId);
    Task<BookEvent?> GetBookEventWithContractAndCustomerAsync(int bookEventId);
    Task UpdateBookEventAsync(BookEvent bookEvent);

    Task<Contract?> GetContractByIdWithCustomerAsync(int contractId);
    Task<Contract?> GetContractByIdWithCustomerAndBookEventAsync(int contractId);
    Task<Contract?> GetContractByIdForDepositAsync(int contractId);
    Task<Contract?> GetContractBySignTokenWithBookEventAsync(string token);
    Task<Contract> CreateContractAndLinkBookEventAsync(Contract contract, BookEvent bookEvent);
    Task SignContractAndUpdateBookEventAsync(Contract contract);
    Task UpdateContractAfterSendSignAsync(Contract contract, string token, DateTime utcNow);
    Task<Payment> AddDepositPaymentAndUpdateContractAsync(Contract contract, Payment payment, decimal totalAmount, decimal depositAmount);

    Task<bool> ExistsPaidDepositForContractAsync(int contractId);
    Task<bool> ExistsByTransactionIdAsync(string? transactionId);
}
