using BusinessObjects.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.DTO.PaymentDTO
{
    public class PayoutViewDTO
    {
        public string PayoutRequestId { get; set; }
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string BankName { get; set; }
        public string BankAccountNumber { get; set; }
        public decimal Amount { get; set; }
        public string Reference { get; set; }
        public DateTime CreatedAt { get; set; }
        public PayoutStatus Status { get; set; }
    }
    public class CreatePayoutRequestDTO
    {
        public string UserId { get; set; }
        public decimal Amount { get; set; }
        public string Reference { get; set; }
    }
    public class ApprovePayoutDTO
    {
        public string PayoutRequestId { get; set; }
        public bool Approve { get; set; }
    }

}
