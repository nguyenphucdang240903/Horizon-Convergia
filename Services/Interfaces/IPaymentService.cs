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
        Task<string> CreateMultiOrderPayOSUrlAsync(CreatePaymentRequestDTO dto, string userId);
        Task<bool> HandlePayOSCallbackAsync(PayOSReturnDTO dto);
        Task<PayoutPagedResultDTO> GetPendingPayoutsAsync(PayoutFilterDTO filter);
        Task<string> ApprovePayoutAsync(ApprovePayoutDTO dto);
    }
}
