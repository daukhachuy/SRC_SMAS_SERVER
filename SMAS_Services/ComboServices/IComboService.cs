using SMAS_BusinessObject.DTOs.Combo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Services.ComboServices
{
    public interface IComboService
    {
        Task<IEnumerable<ComboListResponse>> GetAvailableCombosAsync();
    }
}
