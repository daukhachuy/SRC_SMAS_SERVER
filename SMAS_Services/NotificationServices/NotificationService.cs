using SMAS_BusinessObject.DTOs.ManagerDTO;
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
        public async Task<bool> CreateNotificationAsync(ChangeWorkstaffRequestDTO notification, int userid)
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

        public async Task<IEnumerable<NotificationDto>> GetAllNotificationAsync(int userId)
        {
            var items = await _notificationREPO.GetAllAsync();

            if (!items.Any())
                return Enumerable.Empty<NotificationDto>();

            var notifications = items.Select(n => new NotificationDto
            {
                NotificationId = n.NotificationId,
                UserId = n.UserId,
                SenderId = n.SenderId,
                Title = n.Title,
                Content = n.Content,
                Type = n.Type,
                Severity = n.Severity,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt,
                ReadAt = n.ReadAt
            });
            return notifications.Where(n => n.UserId == userId).ToList();

        }

        public async Task<IEnumerable<NotificationDto>> GetNotificationsByUserIdAsync(int userId)
        {
            var items = await _notificationREPO.GetAllAsync();
            if (!items.Any())
                return Enumerable.Empty<NotificationDto>();

            var notifications = items.Select(n => new NotificationDto
            {
                NotificationId = n.NotificationId,
                UserId = n.UserId,
                SenderId = n.SenderId,
                Title = n.Title,
                Content = n.Content,
                Type = n.Type,
                Severity = n.Severity,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt,
                ReadAt = n.ReadAt
            });

            return notifications.Where(n => n.UserId == userId && n.IsRead == false).ToList();
        }

        public async Task<bool> MarkNotificationAsReadAsync(int notificationId, int userId)
        {
            var notifications = await GetAllNotificationAsync(userId);
            var  notification = notifications.FirstOrDefault(n => n.NotificationId == notificationId);
            if (notification == null || notification.UserId != userId)
                return false;
            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
            var requets = new Notification
            {
                NotificationId = notification.NotificationId,
                UserId = notification.UserId,
                SenderId = notification.SenderId,
                Title = notification.Title,
                Content = notification.Content,
                Type = notification.Type,
                Severity = notification.Severity,
                IsRead = notification.IsRead,
                CreatedAt = notification.CreatedAt,
                ReadAt = notification.ReadAt
            };
            return await _notificationREPO.UpdateNotificationAsync(requets);
        }
    }
}
