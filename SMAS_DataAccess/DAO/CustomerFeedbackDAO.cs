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

        public async Task<List<CustomerFeedback>> GetFeedbackToAnalysisAsync()
        {
            return await _context.CustomerFeedbacks
                                          .Where(f => f.CreatedAt >= DateTime.Now.AddMonths(-3) && f.Comment != null)
                                          .ToListAsync();
        }

        public async Task<bool> CreateFeedbackAsync(CustomerFeedback feedback)
        {
            _context.CustomerFeedbacks.Add(feedback);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateFeedbackAsync(CustomerFeedback feedback)
        {
            var existingFeedback = await _context.CustomerFeedbacks.FirstOrDefaultAsync(f => f.OrderId == feedback.OrderId && f.UserId == feedback.UserId);
            if (existingFeedback == null) return false;
            existingFeedback.Comment = feedback.Comment;
            existingFeedback.Rating = feedback.Rating;
            existingFeedback.FeedbackType = feedback.FeedbackType;
            existingFeedback.UpdatedAt = feedback.UpdatedAt;
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
