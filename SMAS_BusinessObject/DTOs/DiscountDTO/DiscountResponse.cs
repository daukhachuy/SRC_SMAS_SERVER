using SMAS_BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.DTOs.DiscountDTO
{
    public class DiscountResponse
    {
        public int DiscountId { get; set; }

        public string Code { get; set; } = null!;

        public string? Description { get; set; }

        public string DiscountType { get; set; } = null!;

        public decimal Value { get; set; }

        public decimal? MinOrderAmount { get; set; }

        public decimal? MaxDiscountAmount { get; set; }

        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }

        public int? UsageLimit { get; set; }

        public int? UsedCount { get; set; }

        public string? ApplicableFor { get; set; }

        public string? Status { get; set; }

        public DateTime? CreatedAt { get; set; }

        public string CreatedBy { get; set; } = null!;

    }
}
