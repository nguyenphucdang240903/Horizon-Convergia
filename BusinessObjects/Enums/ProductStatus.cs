using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Enums
{
    public enum ProductStatus
    {
        Active,        
        OutOfStock,   
        Suspended,     // lock by admin
    }
}

