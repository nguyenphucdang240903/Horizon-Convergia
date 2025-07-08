using BusinessObjects.DTO.ProductAnalysisDTO;
using Microsoft.Extensions.Configuration;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Services
{
    public class ProductAnalysisService : IProductAnalysisService
    {
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _httpClientFactory;

        public ProductAnalysisService(IConfiguration config, IHttpClientFactory httpClientFactory)
        {
            _config = config;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<GeminiResponseDTO> AnalyzeProductAsync(ProductImageAnalysisDTO dto)
        {
            var apiKey = _config["Gemini:ApiKey"];
            var endpoint = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key={apiKey}";

            // Đọc ảnh ra base64
            string base64Image;
            using (var ms = new MemoryStream())
            {
                await dto.Image.CopyToAsync(ms);
                base64Image = Convert.ToBase64String(ms.ToArray());
            }

            var promptPrefix = @"Bạn là chuyên gia định giá xe máy cũ tại Việt Nam với nhiều năm kinh nghiệm và làm việc cho công ty Horizon Convergia
                                Dựa vào hình ảnh và mô tả sau đây, bạn hãy đưa ra mức giá hợp lý nhất cho chiếc xe trên thị trường hiện tại tại Việt Nam. 
                                Nếu mô tả không đầy đủ hoặc không có, bạn hãy ưu tiên dựa vào hình ảnh để ước tính giá.
                                Chỉ đưa ra một con số giá ước tính tính theo đơn vị VNĐ, giải thích ngắn gọn tại sao lại có giá đó 
                                Nếu thông tin không đủ rõ hoặc không thể định giá, hãy trả lời ""Không đủ thông tin để định giá
                                Thông tin xe:";
            var payload = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new object[]
                        {
                            new { text = $"{promptPrefix}\n{dto.Description}" },
                            new
                            {
                                inline_data = new
                                {
                                    mime_type = "image/jpeg",
                                    data = base64Image
                                }
                            }
                        }
                    }
                }
            };

            var jsonPayload = JsonSerializer.Serialize(payload);
            var client = _httpClientFactory.CreateClient();
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(endpoint, content);
            var rawResponse = await response.Content.ReadAsStringAsync();

            var suggestedPrice = ParsePriceFromGeminiResponse(rawResponse);

            return new GeminiResponseDTO
            {
                SuggestedPrice = suggestedPrice,
                RawResponse = rawResponse
            };
        }

        private string ParsePriceFromGeminiResponse(string rawResponse)
        {
            try
            {
                using var doc = JsonDocument.Parse(rawResponse);
                var text = doc.RootElement
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text")
                    .GetString();

                return text ?? "Không xác định được giá";
            }
            catch
            {
                return "Không xác định được giá";
            }
        }
    }
}
