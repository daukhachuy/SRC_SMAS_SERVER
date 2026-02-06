using SMAS_BusinessObject.DTOs.Feedback;
using SMAS_DataAccess.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Repositories.CustomerFeedbackRepositories
{
    public class CustomerFeedbackRepository : ICustomerFeedbackRepository
    {
        private readonly CustomerFeedbackDAO _customerFeedbackDAO;

        public CustomerFeedbackRepository(CustomerFeedbackDAO customerFeedbackDAO)
        {
            _customerFeedbackDAO = customerFeedbackDAO;
        }

        public async Task<IEnumerable<FeedbackListResponse>> GetAllFeedbacksAsync()
        {
            var feedbacks = await _customerFeedbackDAO.GetAllFeedbacksAsync();

            return feedbacks.Select(f => new FeedbackListResponse
            {
                FeedbackId = f.FeedbackId,
                Fullname = f.User.Fullname,
                Avatar = f.User.Avatar,
                Comment = f.Comment,
                Rating = f.Rating,
                CreatedAt = f.CreatedAt,
                UpdatedAt = f.UpdatedAt
            }).ToList();
        }
    }
}
