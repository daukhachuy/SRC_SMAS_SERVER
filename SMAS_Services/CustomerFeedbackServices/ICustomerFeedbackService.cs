using SMAS_BusinessObject.DTOs.Feedback;
using SMAS_BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Services.CustomerFeedbackServices
{
    public interface ICustomerFeedbackService
    {
        Task<IEnumerable<FeedbackListResponse>> GetAllFeedbacksAsync();

        Task<(bool status , string message)> CreateFeedbackAsync(CreateFeedbackRequest dto ,int userid );
    }
}
