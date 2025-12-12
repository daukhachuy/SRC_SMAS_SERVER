using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.Domain
{
    public class WorkShift
    {
        [Key]
        public int ShiftId { get; set; }

        // FK → User 
        [Required]
        public int UserId { get; set; }

        public DateTime Date { get; set; }

        public bool IsWorking { get; set; } = true;

        public TimeSpan? StartTime { get; set; }

        public TimeSpan? EndTime { get; set; }

        // FK → User (người tạo lịch)
        public int? CreateByUserId { get; set; }

        public bool IsPaid { get; set; } = false;

       
        public virtual User? User { get; set; }

        public virtual User? CreatedByUser { get; set; }
    }
}
