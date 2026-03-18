using SMAS_BusinessObject.DTOs.NotificationDTO;
using SMAS_BusinessObject.Models;
using SMAS_Repositories.Notificationrepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Services.NotificationServices
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationrepository _notificationREPO;
        public NotificationService(INotificationrepository notificationREPO)
        {
            _notificationREPO = notificationREPO;
        }
        public async Task<bool> CreateNotificationAsync(ChangeWorkstaffRequestDTO notification , int userid)
        {
            var request = new Notification
            {
                UserId = userid,
                Title = notification.Title,
                Content = notification.Content,
                CreatedAt = DateTime.UtcNow,
                IsRead = false,
                SenderId = notification.SenderId,
                Type = "Requests",
                Severity = "Warning"
            };
            return await _notificationREPO.CreateNotificationAsync(request);
        }
    }
}
