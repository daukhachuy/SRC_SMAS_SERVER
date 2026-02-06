using Microsoft.EntityFrameworkCore;
using SMAS_BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_DataAccess.DAO
{
    public class CustomerFeedbackDAO
    {
        private readonly RestaurantDbContext _context;

        public CustomerFeedbackDAO(RestaurantDbContext context)
        {
            _context = context;
        }

        public async Task<List<CustomerFeedback>> GetAllFeedbacksAsync()
        {
            return await _context.CustomerFeedbacks.Include(c => c.User).ToListAsync();
        }
    }
}
