using SMAS_BusinessObject.DTOs.IngredientDTO;
using SMAS_Repositories.IngredientReposotories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Services.IngredientServices
{
    public class IngredientService : IIngredientService
    {
        private readonly IIngredientReposotory _ingredientReposotory;

        public IngredientService(IIngredientReposotory ingredientReposotory)
        {
            _ingredientReposotory = ingredientReposotory;
        }

        public async Task<IEnumerable<IngredientResponseDTO>> GetAllIngredientsAsync()
        {
            return await _ingredientReposotory.GetAllIngredientsAsync();
        }
    }
}
