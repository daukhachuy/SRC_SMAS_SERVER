using SMAS_BusinessObject.DTOs.PayOSDTO;
using SMAS_BusinessObject.Models;
using SMAS_DataAccess.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Repositories.OrderRepositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly PaymentDAO _paymentDAO;
        private readonly OrderDAO _orderDAO;

        public PaymentRepository(PaymentDAO paymentDAO, OrderDAO orderDAO)
        {
            _paymentDAO = paymentDAO;
            _orderDAO = orderDAO;
        }

        public async Task<(bool status , string message)> CreatePaymentOrderCashAsync(PaymentOrderCashRequestDTO payment , int userid )
        {
            var existingPayment = await _paymentDAO.CheckPaymentAsync(payment.OrderId);
            if(!existingPayment.isEnough)
            {
                return (false, existingPayment.message);
            }
            var transactionId = $"CASH-{payment.OrderId}-{DateTime.UtcNow.Ticks}";
            var dateTimeNow = DateTime.UtcNow;
            var paymentEntity = new Payment
            {
                OrderId = payment.OrderId,
                Amount = payment.Amount,
                Note = payment.Note ?? "order_cash",
                ReceivedBy = userid,
                PaymentMethod = "Cash",
                PaymentStatus = "Paid",
                CreatedAt = dateTimeNow,
                TransactionId = transactionId,
                PaymentCode = transactionId,
                PaidAt = dateTimeNow

            };
            await _paymentDAO.CreatePaymentCashAsync(paymentEntity);
            var result = await _paymentDAO.CheckPaymentAsync(payment.OrderId);
            if (!result.isEnough)
            {
                await _orderDAO.UpdateOrderStatusAsync(payment.OrderId, "Completed");
            }
            var checkDelivery = await _paymentDAO.IsOrderDeliveryAsync(payment.OrderId);
            
            return (true, "Thanh toán tiền mặt thành công");
        }

        public async Task<(bool success, string message, decimal remaining, int orderId)>
            CreateRemainingPaymentLinkAsync(string orderCode, string returnUrl, string cancelUrl)
        {
            var order = await _paymentDAO.GetOrderWithPaymentsByCodeAsync(orderCode);
            if (order == null)
                return (false, "Không tìm thấy đơn hàng.", 0, 0);

            var check = await _paymentDAO.CheckPaymentAsync(order.OrderId);
            if (!check.isEnough)
                return (false, "Đơn hàng đã thanh toán đủ.", 0, order.OrderId);

            var paidAmount = order.Payments
                .Where(p => p.PaymentStatus == "Paid")
                .Sum(p => p.Amount);
            var remaining = order.TotalAmount - paidAmount;

            if (remaining <= 0)
                return (false, "Đơn hàng đã thanh toán đủ.", 0, order.OrderId);

            return (true, "OK", remaining, order.OrderId);
        }

    }
}
