using SMAS_BusinessObject.DTOs.PayOSDTO;
using SMAS_BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Repositories.OrderRepositories
{
    public interface IPaymentRepository
    {
        Task<(bool status, string message)> CreatePaymentOrderCashAsync(PaymentOrderCashRequestDTO payment, int userid);

        Task<(bool success, string message, decimal remaining, int orderId)> CreateRemainingPaymentLinkAsync(
            string orderCode, string returnUrl, string cancelUrl);

        Task<List<TransactionHistoryItemDTO>> GetTransactionHistoryAsync(TransactionHistoryRequestDTO request);
    }
}
