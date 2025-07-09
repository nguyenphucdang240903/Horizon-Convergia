using BusinessObjects.Enums;
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
        public string PaymentMethod { get; set; } = "BankTransfer";
        public string Description { get; set; }
    }
    public class PayOSReturnDTO
    {
        public string OrderCode { get; set; }
        public string Status { get; set; }
        public string id { get; set; }
    }

    public class SendPaymentLinkDTO
    {
        public string ProductId { get; set; }
    }
    public class CreatePayoutDTO
    {
        public string UserId { get; set; }
        public decimal Amount { get; set; }
        public string Reference { get; set; } // e.g. ORDER-{orderId}
    }

    public class ApprovePayoutDTO
    {
        public string PaymentId { get; set; }
        public bool Approve { get; set; }
    }

    public class PayoutViewDTO
    {
        public string PaymentId { get; set; }
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string BankName { get; set; }
        public string BankAccountNumber { get; set; }
        public string BankAccountName { get; set; }
        public decimal Amount { get; set; }
        public string Reference { get; set; }
        public DateTime CreatedAt { get; set; }
        public PaymentStatus Status { get; set; }
    }


}
