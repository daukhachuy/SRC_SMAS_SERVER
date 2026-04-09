using SMAS_BusinessObject.DTOs.Feedback;
using SMAS_Repositories.CustomerFeedbackRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Services.CustomerFeedbackServices
{
    public class CustomerFeedbackService : ICustomerFeedbackService
    {
        private readonly ICustomerFeedbackRepository _customerFeedbackRepository;

        public CustomerFeedbackService(ICustomerFeedbackRepository customerFeedbackRepository)
        {
            _customerFeedbackRepository = customerFeedbackRepository;
        }

        public async Task<IEnumerable<FeedbackListResponse>> GetAllFeedbacksAsync()
        {
            return await _customerFeedbackRepository.GetAllFeedbacksAsync();
        }

        public async Task<(bool status, string message)> CreateFeedbackAsync(CreateFeedbackRequest dto, int userid)
        {
            return await _customerFeedbackRepository.CreateFeedbackAsync(dto, userid);
        }
    }
}
