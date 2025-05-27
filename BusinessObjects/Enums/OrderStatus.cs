using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Enums
{
    public enum OrderStatus
    {
        Pending,          // wating to chose shipper
        Confirmed,        // comfirmed by admin
        Processing,       
        Shipping,         
        Delivered,        
        Cancelled,        
        Returned,        
        Refunded         
    }
}

