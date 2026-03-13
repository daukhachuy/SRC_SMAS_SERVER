using SMAS_BusinessObject.DTOs.IngredientDTO;
using SMAS_DataAccess.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Repositories.IngredientReposotories
{
    public class IngredientReposotory : IIngredientReposotory
    {
        private readonly IngredientDAO _ingredientDAO;

        public IngredientReposotory(IngredientDAO ingredientDAO)
        {
            _ingredientDAO = ingredientDAO;
        }


        public async Task<IEnumerable<IngredientResponseDTO>> GetAllIngredientsAsync()
        {
            var ingredients = await _ingredientDAO.GetAllIngredientsAsync();
            return ingredients.Select(i => new IngredientResponseDTO
            {
                IngredientId = i.IngredientId,
                IngredientName = i.IngredientName,
                UnitOfMeasurement = i.UnitOfMeasurement,
                WarningLevel = i.WarningLevel,
                CurrentStock = i.Inventories?.Sum(inv => inv.QuantityOnHand),
                Description = i.Description,
                CreatedAt = i.CreatedAt,
                UpdatedAt = i.UpdatedAt,
                IsActive = i.IsActive,
            }).ToList();
        }
    }
}
