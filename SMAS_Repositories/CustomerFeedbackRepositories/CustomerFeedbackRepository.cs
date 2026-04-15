using SMAS_BusinessObject.DTOs.Feedback;
using SMAS_BusinessObject.Models;
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
        private readonly OrderDAO _orderDAO;
        public CustomerFeedbackRepository(CustomerFeedbackDAO customerFeedbackDAO , OrderDAO orderDAO)
        {
            _customerFeedbackDAO = customerFeedbackDAO;
            _orderDAO = orderDAO;
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

        public async Task<(bool status, string message)> CreateFeedbackAsync(CreateFeedbackRequest dto, int userid)
        {
            var order = await _orderDAO.GetOrderByCodeNoTrackingAsync(dto.OrderCode);
            if (order == null || order.UserId != userid)
            {
                return (false, "không tìm thấy đơn hàng của bạn");
            }
            else if (order.OrderItems == null || !order.OrderItems.Any())
            {
                return (false, "Đơn hàng không có món ăn nào để đánh giá");
            }
            else if (order.OrderStatus != "Completed")
            {
                return (false, "Chỉ có thể đánh giá đơn hàng đã hoàn thành");
            }
            var feedback = new CustomerFeedback
            {
                UserId = userid,
                OrderId = order.OrderId,
                Comment = dto.Comment,
                FeedbackType = dto.FeedbackType,
                Rating = dto.Rating,
            };
            if (order.CustomerFeedbacks.Any())
            {
                feedback.UpdatedAt = DateTime.UtcNow;
                var result1 = await _customerFeedbackDAO.UpdateFeedbackAsync(feedback);
                if (!result1) return (false, "Tạo phản hồi thất bại");
                return (true, "Tạo phản hồi thành công");
            }
            feedback.CreatedAt = DateTime.UtcNow;
            var result = await _customerFeedbackDAO.CreateFeedbackAsync(feedback);
            if (!result) return (false,"Tạo phản hồi thất bại");
            return (true,"Tạo phản hồi thành công");

        }

        public async Task<IEnumerable<CustomerFeedback>> GetFeedbackToAnalysisAsync()
        {
            return await _customerFeedbackDAO.GetFeedbackToAnalysisAsync();

        }
    }
}
