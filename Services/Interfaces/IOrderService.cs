using BusinessObjects.DTO.OrderDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IOrderService
    {
        Task<string> CreateOrderAsync(CreateOrderDTO orderDto);
    }

}
