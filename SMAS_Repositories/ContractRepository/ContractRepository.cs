using SMAS_BusinessObject.DTOs.ContractDTO;
using SMAS_BusinessObject.Models;
using SMAS_DataAccess.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Repositories.ContractRepository
{
    public class ContractRepository : IContractRepository
    {
        private readonly ContractDAO _contractDAO;

        public ContractRepository(ContractDAO contractDAO)
        {
            _contractDAO = contractDAO;
        }

        public async Task<ContractResponseDTO?> GetContractByBookEventCodeAsync(string bookingCode)
        {
            var contract = await _contractDAO.GetContractByBookEventCodeAsync(bookingCode);

            if (contract == null)
                return null;

            return new ContractResponseDTO
            {
                ContractId = contract.ContractId,
                ContractCode = contract.ContractCode,
                CustomerId = contract.CustomerId,
                CustomerName = contract.Customer?.Fullname,
                CustomerEmail = contract.Customer?.Email,
                CustomerPhone = contract.Customer?.Phone,

                BookEventId = contract.BookEventId,
                BookingCode = contract.BookEvent?.BookingCode,
                EventName = contract.BookEvent?.Event?.Title,

                EventType = contract.EventType,
                EventDate = contract.EventDate,
                NumberOfGuests = contract.NumberOfGuests,
                TotalAmount = contract.TotalAmount,
                DepositAmount = contract.DepositAmount,
                RemainingAmount = contract.RemainingAmount,
                SignMethod = contract.SignMethod,
                SignedAt = contract.SignedAt,
                ContractFileUrl = contract.ContractFileUrl,
                ServiceDetails = contract.ServiceDetails,
                TermsAndConditions = contract.TermsAndConditions,
                Status = contract.Status,
                CreatedAt = contract.CreatedAt,
                UpdatedAt = contract.UpdatedAt,

                Payments = contract.Payments.Select(p => new PaymentSummaryDTO
                {
                    PaymentId = p.PaymentId,
                    Amount = p.Amount,
                    PaymentMethod = p.PaymentMethod,
                    PaymentStatus = p.PaymentStatus,
                    PaidAt = p.PaidAt
                }).ToList()
            };
        } }
    }
