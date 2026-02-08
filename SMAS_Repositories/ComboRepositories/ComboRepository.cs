using SMAS_BusinessObject.DTOs.Combo;
using SMAS_DataAccess.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Repositories.ComboRepositories
{
    public class ComboRepository : IComboRepository
    {
        private readonly ComboDAO _comboDAO;

        public ComboRepository(ComboDAO comboDAO)
        {
            _comboDAO = comboDAO;
        }

        public async Task<IEnumerable<ComboListResponse>> GetAvailableComboListAsync()
        {
            var combos = await _comboDAO.GetAvailableCombosWithFoodsAsync();

            return combos.Select(c => new ComboListResponse
            {
                ComboId = c.ComboId,
                Name = c.Name,
                Description = c.Description,
                Price = c.Price,
                DiscountPercent = c.DiscountPercent,
                Image = c.Image,
                StartDate = c.StartDate,
                ExpiryDate = c.ExpiryDate,
                NumberOfUsed = c.NumberOfUsed,
                MaxUsage = c.MaxUsage,
                IsAvailable = c.IsAvailable,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt,

                Foods = c.ComboFoods.Select(cf => new ComboFoodItemDto
                {
                    FoodId = cf.FoodId,
                    FoodName = cf.Food.Name,
                    FoodImage = cf.Food.Image,
                    Quantity = cf.Quantity,
                    FoodPrice = cf.Food.Price   // tùy chọn: lấy giá món gốc
                }).ToList()
            });
        }
    }
}
