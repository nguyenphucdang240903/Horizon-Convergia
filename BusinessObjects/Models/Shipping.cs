using BusinessObjects.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Models
{
    public class Shipping
    {
        public long Id { get; set; }
        public long CarrierId { get; set; }
        public string TrackingNumber { get; set; }
        public decimal ShippingFee { get; set; }
        public DateTime? ActualDeliveryDate { get; set; }
        public ShippingStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public long OrderId { get; set; }
        public Order Order { get; set; }
        public long UserId { get; set; }
        public User User { get; set; }

    }

}
