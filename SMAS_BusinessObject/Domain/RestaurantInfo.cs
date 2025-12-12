using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.Domain
{
    public class RestaurantInfo
    {
        public int RestaurantInfoId { get; set; }     // PK

        public string Address { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string? ImageLogo { get; set; }

        public TimeSpan OpeningTime { get; set; }

        public TimeSpan ClosingTime { get; set; }

        public string Status { get; set; } = string.Empty;

        public string? LinkURL { get; set; }

        public decimal DeliveryRadius { get; set; }

        public string NameRestaurant { get; set; } = string.Empty;

        public int UserId { get; set; }               // FK
    }
}
