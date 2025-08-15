using BusinessObjects.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using System.Security.Claims;

namespace HorizonConvergia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("admin")]
        //[Authorize(Policy = "Admin")]
        public async Task<IActionResult> GetAdminDashboard([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var result = await _dashboardService.GetAdminDashboardAsync(startDate, endDate);
            return Ok(result);
        }

        [HttpGet("seller")]
        //[Authorize(Policy = "Seller")]
        public async Task<IActionResult> GetSellerDashboard([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var sellerId = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            var result = await _dashboardService.GetSellerDashboardAsync(sellerId, startDate, endDate);
            return Ok(result);
        }
    }

}
