using SMAS_BusinessObject.DTOs.DiscountDTO;
using SMAS_BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Repositories.DiscountRepositories
{
    public interface IDiscountRepository
    {
        Task<IEnumerable<DiscountResponse>> GetAllDiscountsAsync();
    }
}
