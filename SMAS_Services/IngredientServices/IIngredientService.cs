using SMAS_BusinessObject.DTOs.IngredientDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Services.IngredientServices
{
    public interface IIngredientService
    {
        Task<IEnumerable<IngredientResponseDTO>> GetAllIngredientsAsync();
    }
}
