using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.DTO.ShippingDTO
{
    public class UnassignedOrdersFilterRequest
    {
        public string? BuyerName { get; set; }
        public string? SellerName { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class AssignShipperRequest
    {
        public string OrderId { get; set; }
        public string CarrierId { get; set; } 
    }
}
