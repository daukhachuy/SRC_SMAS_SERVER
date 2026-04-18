using SMAS_BusinessObject.DTOs.ManagerDTO;
using SMAS_BusinessObject.DTOs.NotificationDTO;
using SMAS_BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Services.NotificationServices
{
    public interface INotificationService
    {
        Task<bool> CreateNotificationAsync(ChangeWorkstaffRequestDTO notification, int userid);
        Task<IEnumerable<NotificationDto>> GetAllNotificationAsync(int userId);
        Task<IEnumerable<NotificationDto>> GetNotificationsByUserIdAsync(int userId);
        Task<bool> MarkNotificationAsReadAsync(int notificationId, int userId);
        Task<bool> CreateAutoNotificationAsync(int userId, int? senderId, string title, string content, string type, string severity = "Information");
    }
}
