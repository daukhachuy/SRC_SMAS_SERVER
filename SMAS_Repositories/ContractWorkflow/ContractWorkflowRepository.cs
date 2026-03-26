using SMAS_BusinessObject.Models;
using SMAS_DataAccess.DAO;

namespace SMAS_Repositories.ContractWorkflow;

public class ContractWorkflowRepository : IContractWorkflowRepository
{
    private readonly BookEventDAO _bookEventDAO;
    private readonly ContractDAO _contractDAO;
    private readonly PaymentDAO _paymentDAO;

    public ContractWorkflowRepository(
        BookEventDAO bookEventDAO,
        ContractDAO contractDAO,
        PaymentDAO paymentDAO)
    {
        _bookEventDAO = bookEventDAO;
        _contractDAO = contractDAO;
        _paymentDAO = paymentDAO;
    }

    public Task<BookEvent?> GetBookEventForReviewAsync(int bookEventId) =>
        _bookEventDAO.GetBookEventForReviewAsync(bookEventId);

    public Task<BookEvent?> GetBookEventForCreateContractAsync(int bookEventId) =>
        _bookEventDAO.GetBookEventForCreateContractAsync(bookEventId);

    public Task<BookEvent?> GetBookEventForDetailAsync(int bookEventId) =>
        _bookEventDAO.GetBookEventForDetailAsync(bookEventId);

    public Task<BookEvent?> GetBookEventWithContractAndCustomerAsync(int bookEventId) =>
        _bookEventDAO.GetBookEventWithContractAndCustomerAsync(bookEventId);

    public Task UpdateBookEventAsync(BookEvent bookEvent) =>
        _bookEventDAO.UpdateBookEventAsync(bookEvent);

    public Task<Contract?> GetContractByIdWithCustomerAsync(int contractId) =>
        _contractDAO.GetByIdWithCustomerAsync(contractId);

    public Task<Contract?> GetContractByIdWithCustomerAndBookEventAsync(int contractId) =>
        _contractDAO.GetByIdWithCustomerAndBookEventAsync(contractId);

    public Task<Contract?> GetContractByIdForDepositAsync(int contractId) =>
        _contractDAO.GetByIdForDepositAsync(contractId);

    public Task<Contract?> GetContractBySignTokenWithBookEventAsync(string token) =>
        _contractDAO.GetBySignTokenWithBookEventAsync(token);

    public Task<Contract> CreateContractAndLinkBookEventAsync(Contract contract, BookEvent bookEvent) =>
        _contractDAO.CreateContractAndLinkBookEventAsync(contract, bookEvent);

    public Task SignContractAndUpdateBookEventAsync(Contract contract) =>
        _contractDAO.SignContractAndUpdateBookEventAsync(contract);

    public Task UpdateContractAfterSendSignAsync(Contract contract, string token, DateTime utcNow) =>
        _contractDAO.UpdateContractAfterSendSignAsync(contract, token, utcNow);

    public Task<Payment> AddDepositPaymentAndUpdateContractAsync(
        Contract contract,
        Payment payment,
        decimal totalAmount,
        decimal depositAmount) =>
        _contractDAO.AddDepositPaymentAndUpdateContractAsync(contract, payment, totalAmount, depositAmount);

    public Task<bool> ExistsPaidDepositForContractAsync(int contractId) =>
        _paymentDAO.ExistsPaidDepositForContractAsync(contractId);

    public Task<bool> ExistsByTransactionIdAsync(string? transactionId) =>
        _paymentDAO.ExistsByTransactionIdAsync(transactionId);
}
