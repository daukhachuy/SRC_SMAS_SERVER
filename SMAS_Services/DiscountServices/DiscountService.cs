using SMAS_BusinessObject.DTOs.DiscountDTO;
using SMAS_Repositories.DiscountRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Services.DiscountServices
{
    public class DiscountService : IDiscountService
    {
        private readonly IDiscountRepository _discountRepository;

        public DiscountService(IDiscountRepository discountRepository)
        {
            _discountRepository = discountRepository;
        }

        public async Task<IEnumerable<DiscountResponse>> GetAllDiscountsAsync()
        {
           return await _discountRepository.GetAllDiscountsAsync();
        }
    }
}
