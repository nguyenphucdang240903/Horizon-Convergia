using BusinessObjects.DTO.DashBoardDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardStatsDTO> GetAdminDashboardAsync(DateTime? startDate, DateTime? endDate);
        Task<DashboardStatsDTO> GetSellerDashboardAsync(string sellerId, DateTime? startDate, DateTime? endDate);
    }

}
