using SMAS_BusinessObject.DTOs.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Repositories.ServiceRepositories
{
    public interface IServiceRepository
    {
        Task<IEnumerable<ServiceListResponse>> GetAllServicesAsync();
    }

}
