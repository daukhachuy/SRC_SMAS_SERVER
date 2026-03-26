using Microsoft.EntityFrameworkCore;
using SMAS_BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_DataAccess.DAO
{
    public class ContractDAO
    {
        private readonly RestaurantDbContext _context;

        public ContractDAO(RestaurantDbContext context)
        {
            _context = context;
        }
        /// Lấy Contract theo BookingCode của BookEvent
      
        public async Task<Contract?> GetContractByBookEventCodeAsync(string bookingCode)
        {
            return await _context.Contracts
                .Include(c => c.Customer)
                .Include(c => c.BookEvent)
                    .ThenInclude(be => be!.Event)
                .Include(c => c.Payments)
                .FirstOrDefaultAsync(c =>
                    c.BookEvent != null &&
                    c.BookEvent.BookingCode == bookingCode);
        }

        public async Task<Contract?> GetByIdWithCustomerAsync(int contractId)
        {
            return await _context.Contracts
                .Include(c => c.Customer)
                .FirstOrDefaultAsync(c => c.ContractId == contractId);
        }

        public async Task<Contract?> GetByIdWithCustomerAndBookEventAsync(int contractId)
        {
            return await _context.Contracts
                .Include(c => c.Customer)
                .Include(c => c.BookEvent)
                .FirstOrDefaultAsync(c => c.ContractId == contractId);
        }

        public async Task<Contract?> GetBySignTokenWithBookEventAsync(string token)
        {
            return await _context.Contracts
                .Include(c => c.BookEvent)
                .FirstOrDefaultAsync(c => c.SignMethod == token);
        }

        public async Task<Contract> CreateContractAndLinkBookEventAsync(Contract contract, BookEvent bookEvent)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Contracts.Add(contract);
                await _context.SaveChangesAsync();

                bookEvent.ContractId = contract.ContractId;
                bookEvent.IsContract = true;
                bookEvent.UpdatedAt = DateTime.UtcNow;
                _context.BookEvents.Update(bookEvent);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return contract;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Contract?> GetByIdForDepositAsync(int contractId)
        {
            return await _context.Contracts
                .Include(c => c.Payments)
                .FirstOrDefaultAsync(c => c.ContractId == contractId);
        }

        public async Task<Payment> AddDepositPaymentAndUpdateContractAsync(
            Contract contract,
            Payment payment,
            decimal totalAmount,
            decimal depositAmount)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Payments.Add(payment);
                contract.Status = "Deposited";
                contract.RemainingAmount = totalAmount - depositAmount;
                contract.UpdatedAt = DateTime.UtcNow;
                _context.Contracts.Update(contract);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return payment;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task SignContractAndUpdateBookEventAsync(Contract contract)
        {
            if (contract.BookEventId == null)
                throw new InvalidOperationException("BookEventId is required.");

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var be = contract.BookEvent ?? await _context.BookEvents.FirstOrDefaultAsync(b => b.BookEventId == contract.BookEventId);
                if (be == null)
                    throw new InvalidOperationException("BookEvent not found.");

                contract.SignedAt = DateTime.UtcNow;
                contract.Status = "Signed";
                contract.UpdatedAt = DateTime.UtcNow;
                _context.Contracts.Update(contract);

                be.Status = "Confirmed";
                be.UpdatedAt = DateTime.UtcNow;
                _context.BookEvents.Update(be);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task UpdateContractAfterSendSignAsync(Contract contract, string token, DateTime utcNow)
        {
            contract.SignMethod = token;
            contract.Status = "Sent";
            contract.UpdatedAt = utcNow;
            _context.Contracts.Update(contract);
            await _context.SaveChangesAsync();
        }
    }
}
