using SMAS_BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.DTOs.Feedback
{
    public class FeedbackListResponse
    {
        public int FeedbackId { get; set; }

        public string Fullname { get; set; } = null!;

        public string? Avatar { get; set; }

        public int Rating { get; set; }

        public string? Comment { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

    }
}
