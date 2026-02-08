using SMAS_BusinessObject.DTOs.DiscountDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Services.DiscountServices
{
    public interface IDiscountService
    {
        Task<IEnumerable<DiscountResponse>> GetAllDiscountsAsync();
    }
}
