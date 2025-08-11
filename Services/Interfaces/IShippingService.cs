using BusinessObjects.DTO.ShippingDTO;
using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IShippingService
    {
        Task<Shipping?> AssignShipperAsync(string shippingId, string carrierId);
        Task<object> GetUnassignedOrdersAsync(UnassignedOrdersFilterRequest filter);
    }
}
