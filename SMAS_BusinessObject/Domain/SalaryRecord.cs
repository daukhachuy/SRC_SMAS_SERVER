using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.Domain
{
    public class SalaryRecord
    {
        public int SalaryRecordId { get; set; }     // PK

        public int StaffId { get; set; }            // FK

        public int Year { get; set; }

        public int Month { get; set; }

        public decimal SalaryPerHours { get; set; }

        public decimal Bonus { get; set; }

        public string? Note { get; set; }

        public decimal Penalty { get; set; }

        public decimal TotalHours { get; set; }

        public int TotalDay { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public int UpdateByUserId { get; set; }     // FK
    }
}
