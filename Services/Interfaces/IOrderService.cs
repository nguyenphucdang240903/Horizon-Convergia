using BusinessObjects.DTO.OrderDTO;
using BusinessObjects.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IOrderService
    {
        Task<List<string>> CreateOrderAsync(CreateOrderFromCartDTO dto, string buyerId);
        Task<PagedResult<OrderListDTO>> SearchOrdersAsync(OrderSearchDTO searchDto);
        Task<OrderDetailDTO> GetOrderDetailAsync(string orderId);
        Task<bool> UpdateOrderStatusAsync(string orderId, OrderStatus newStatus);
    }

}
