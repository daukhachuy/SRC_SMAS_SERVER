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
        Task<IEnumerable<NotificationDto>> GetAllNotificationAsync();
    }
}
