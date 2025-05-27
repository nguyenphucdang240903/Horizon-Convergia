using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Enums
{
    public enum PaymentStatus
    {
        Pending,        
        Processing,    
        Completed,     
        Failed,        
        Cancelled,      
        Refunded        
    }
}
