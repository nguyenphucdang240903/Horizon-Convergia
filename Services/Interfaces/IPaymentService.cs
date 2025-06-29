using BusinessObjects.DTO.PaymentDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IPaymentService
    {
        Task<string> CreatePayOSUrlAsync(CreatePaymentRequestDTO dto, string userId);
        Task<bool> HandlePayOSCallbackAsync(PayOSReturnDTO dto);

    }
}
