using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.DTO.CartDTO
{
    public class CreateCartDTO
    {
        public string Status { get; set; }
        public decimal Discount { get; set; } = 0;
        public int Quantity { get; set; }
    }

    public class UpdateCartDTO : CreateCartDTO
    {
        public int Quantity { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CartCreateResult()
    {
        public CartDTO? Cart { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
