using SMAS_BusinessObject.DTOs.Combo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Repositories.ComboRepositories
{
    public interface IComboRepository
    {
        Task<IEnumerable<ComboListResponse>> GetAvailableComboListAsync();

        Task<bool> UpdateStatusByComboId(int comboId);
    }
}
