using SMAS_BusinessObject.Models;
using SMAS_DataAccess.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Repositories.Notificationrepositories
{
    public class Notificationrepository : INotificationrepository
    {
        private readonly NotificationDAO _notificationDAO;

        public Notificationrepository(NotificationDAO notificationDAO)
        {
            _notificationDAO = notificationDAO;
        }
        public async Task<bool> CreateNotificationAsync(Notification notification)
        {
            return await _notificationDAO.CreateNotificationAsync(notification);
        }
    }
}
