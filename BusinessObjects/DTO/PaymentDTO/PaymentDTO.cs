using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.DTO.PaymentDTO
{
    public class CreatePaymentRequestDTO
    {
        public string OrderId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = "BankTransfer";
        public string Description { get; set; }

        public string ReturnUrl { get; set; }
    }
    public class PayOSReturnDTO
    {
        public string OrderCode { get; set; }
        public string Status { get; set; }
        public string TransactionId { get; set; }
    }

}
