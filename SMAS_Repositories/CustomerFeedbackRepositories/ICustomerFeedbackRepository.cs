using SMAS_BusinessObject.DTOs.Feedback;
using SMAS_BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Repositories.CustomerFeedbackRepositories
{
    public interface ICustomerFeedbackRepository
    {
        Task<IEnumerable<FeedbackListResponse>> GetAllFeedbacksAsync();

        Task<IEnumerable<CustomerFeedback>> GetFeedbackToAnalysisAsync();
    }
}
