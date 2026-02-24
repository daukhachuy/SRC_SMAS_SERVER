using SMAS_BusinessObject.DTOs.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Services.ServiceServices
{
    public interface IServiceService
    {
        Task<IEnumerable<ServiceListResponse>> GetAllServicesAsync();
    }

}
