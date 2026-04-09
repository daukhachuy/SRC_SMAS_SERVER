using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.DTOs.Feedback
{
    public class CreateFeedbackRequest
    {

        [Required(ErrorMessage = "OrderCode không được để trống")]
        [StringLength(50, ErrorMessage = "OrderCode tối đa 50 ký tự")]
        public string OrderCode { get; set; }

        [Required(ErrorMessage = "Rating không được để trống")]
        [Range(1, 5, ErrorMessage = "Rating phải từ 1 đến 5")]
        public int Rating { get; set; }

        [StringLength(500, ErrorMessage = "Comment tối đa 500 ký tự")]
        public string? Comment { get; set; }

        [StringLength(100, ErrorMessage = "FeedbackType tối đa 100 ký tự")]
        public string? FeedbackType { get; set; }
    }
}
