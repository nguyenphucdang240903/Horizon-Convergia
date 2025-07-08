using BusinessObjects.DTO.ProductAnalysisDTO;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace HorizonConvergia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductAnalysisController : ControllerBase
    {
        private readonly IProductAnalysisService _service;

        public ProductAnalysisController(IProductAnalysisService service)
        {
            _service = service;
        }

        [HttpPost("analyze")]
        public async Task<IActionResult> AnalyzeProduct([FromForm] ProductImageAnalysisDTO dto)
        {
            var result = await _service.AnalyzeProductAsync(dto);
            return Ok(result);
        }
    }
}
