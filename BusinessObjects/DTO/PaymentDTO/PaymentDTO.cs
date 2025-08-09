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
        public List<string> OrderIds { get; set; }
        public string PaymentMethod { get; set; } 
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
        public string Reference { get; set; } 
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
    public class PayoutFilterDTO
    {
        public string? FullName { get; set; }
        public string? BankName { get; set; }
        public string? BankAccountNumber { get; set; }
        public string? BankAccountName { get; set; }
        public string? Reference { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
    public class RefundRequestDTO
    {
        public string PaymentId { get; set; }
        public string Reason { get; set; }
    }
    public class RefundRequestViewDTO
    {
        public string PaymentId { get; set; }
        public string OrderId { get; set; }
        public string OrderCode { get; set; }
        public decimal Amount { get; set; }
        public string BuyerName { get; set; }
        public string Email { get; set; }
        public DateTime RequestedAt { get; set; }
        public string Reason { get; set; }
    }
    public class RefundFilterDTO
    {
        public string? BuyerName { get; set; }
        public string? OrderCode { get; set; }
        public string? Email { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
