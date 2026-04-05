using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAS_BusinessObject.DTOs.PayOSDTO
{
    public class PaymentOrderCashRequestDTO
    {

        [Required(ErrorMessage = "OrderId không được để trống")]
        public int OrderId { get; set; }

        [Required(ErrorMessage = "Số tiền không được để trống")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount phải lớn hơn 0")]
        public decimal Amount { get; set; }

        [StringLength(500, ErrorMessage = "Note không được vượt quá 500 ký tự")]
        public string? Note { get; set; }
    }

    public class PaymentContractCashRequestDTO
    {

        [Required(ErrorMessage = "ContractId không được để trống")]
        public int ContractId { get; set; }

        [Required(ErrorMessage = "Số tiền không được để trống")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount phải lớn hơn 0")]
        public decimal Amount { get; set; }

        [StringLength(500, ErrorMessage = "Note không được vượt quá 500 ký tự")]
        public string? Note { get; set; }

    }
}
