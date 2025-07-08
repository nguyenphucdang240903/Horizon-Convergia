using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.DTO.ProductAnalysisDTO
{
    public class ProductImageAnalysisDTO
    {
        public IFormFile Image { get; set; }
        public string Description { get; set; }
    }
    public class GeminiResponseDTO
    {
        public string SuggestedPrice { get; set; }
        public string RawResponse { get; set; }
    }
}
