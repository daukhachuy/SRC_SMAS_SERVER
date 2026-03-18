using SMAS_BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Repositories.Notificationrepositories
{
    public interface INotificationrepository
    {
        Task<bool> CreateNotificationAsync(Notification notification);
    }
}
