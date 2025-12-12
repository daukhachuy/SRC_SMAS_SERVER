using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.Domain
{
    public class Ingredients
    {
        public int IngredientId { get; set; }
        public string IngredientName { get; set; } = string.Empty;
        public string UnitOfMeasurement { get; set; } = string.Empty;
        public int WarningLevel { get; set; }
        public DateTime CreateAt { get; set; }
        public string? Description { get; set; }
        public DateTime UpdateAt { get; set; }
    }
}
