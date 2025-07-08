using BusinessObjects.DTO.ProductAnalysisDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IProductAnalysisService
    {
        Task<GeminiResponseDTO> AnalyzeProductAsync(ProductImageAnalysisDTO dto);
    }
}
