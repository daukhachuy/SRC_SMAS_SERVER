using SMAS_BusinessObject.DTOs.IngredientDTO;
using SMAS_BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_Repositories.IngredientReposotories
{
    public interface IIngredientReposotory
    {
        Task<IEnumerable<IngredientResponseDTO>> GetAllIngredientsAsync();
    }
}
