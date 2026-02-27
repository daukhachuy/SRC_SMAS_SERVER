using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.DTOs.Food
{
    public class FoodFilterRequestDTO
    {
        public List<int>? CategoryIds { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "MinPrice phải lớn hơn hoặc bằng 0")]
        public decimal? MinPrice { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "MaxPrice phải lớn hơn hoặc bằng 0")]
        public decimal? MaxPrice { get; set; }
    }
}
