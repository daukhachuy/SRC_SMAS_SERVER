using Microsoft.EntityFrameworkCore;
using SMAS_BusinessObject.Models;

namespace SMAS_DataAccess.DAO;

public class PaymentDAO
{
    private readonly RestaurantDbContext _context;

    public PaymentDAO(RestaurantDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ExistsPaidDepositForContractAsync(int contractId)
    {
        return await _context.Payments.AnyAsync(p =>
            p.ContractId == contractId &&
            p.Note == "deposit" &&
            p.PaymentStatus == "Paid");
    }
}
