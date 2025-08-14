using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.DTO.DashBoardDTO
{
    public class DashboardStatsDTO
    {
        public decimal TotalRevenue { get; set; }
        public int TotalProducts { get; set; }
        public int TotalOrders { get; set; }
        public List<TransactionDTO> Transactions { get; set; }
    }

    public class TransactionDTO
    {
        public string Id { get; set; }
        public string Reference { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentStatus { get; set; }
    }

}
