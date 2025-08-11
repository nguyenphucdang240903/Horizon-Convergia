using BusinessObjects.Models;
using DataAccessObjects.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
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
    public class ChatService : IChatService
    {
        private readonly IMessageService _messageService;
        private readonly IHttpClientFactory _httpFactory;
        private readonly IConfiguration _config;
        private readonly IHubContext<ChatHub> _hub;
        private readonly AppDbContext _db;

        // config
        private readonly int _historyLimit;
        private readonly int _chunkSize;
        private readonly int _timeoutSeconds;
        private readonly string _aiBotId; // ID của bot từ config

        public ChatService(IMessageService messageService,
                           IHttpClientFactory httpFactory,
                           IConfiguration config,
                           IHubContext<ChatHub> hub,
                           AppDbContext db)
        {
            _messageService = messageService;
            _httpFactory = httpFactory;
            _config = config;
            _hub = hub;
            _db = db;

            _historyLimit = int.TryParse(_config["Chat:HistoryLimit"], out var h) ? h : 8;
            _chunkSize = int.TryParse(_config["Chat:ChunkSize"], out var c) ? c : 200;
            _timeoutSeconds = int.TryParse(_config["Chat:TimeoutSeconds"], out var t) ? t : 30;

            _aiBotId = _config["Chat:AIBotId"] ?? "AI_BOT";
        }

        public async Task<Message> HandleUserMessageAsync(
            string senderId,
            string receiverId,
            string content,
            IFormFile? image = null,
            CancellationToken ct = default)
        {
            // 1. Convert ảnh sang Base64 nếu có
            string? imageBase64 = null;
            if (image != null)
            {
                using var ms = new MemoryStream();
                await image.CopyToAsync(ms, ct);
                imageBase64 = Convert.ToBase64String(ms.ToArray());
            }

            // 2. Lưu tin nhắn user
            var userMessage = new Message
            {
                Content = content ?? "",
                CreatedAt = DateTime.UtcNow,
                ImageBase64 = imageBase64,
                AttachmentUrl = null,
                MessageType = image != null ? MessageType.Image : MessageType.Text,
                SenderId = senderId,
                ReceiverId = receiverId,
                Status = MessageStatus.Sent
            };

            await _messageService.AddAsync(userMessage, ct);

            // 3. Broadcast tin user tới group
            var groupName = GetGroupName(senderId, receiverId);
            await _hub.Clients.Group(groupName).SendAsync("ReceiveMessage", ToDto(userMessage), ct);

            // 4. Nếu là chat với AI → gọi AI xử lý
            if (receiverId.Equals(_aiBotId, StringComparison.OrdinalIgnoreCase))
            {
                return await HandleAiResponseAsync(senderId, receiverId, content, imageBase64, ct);
            }

            return userMessage;
        }

        private async Task<Message> HandleAiResponseAsync(
    string senderId,
    string receiverId,
    string content,
    string? imageBase64,
    CancellationToken ct)
        {
            // 1. Lấy lịch sử chat
            var recent = (await _messageService.GetSessionHistoryByParticipantsAsync(senderId, receiverId, _historyLimit, ct)).ToList();

            // 2. Gọi Gemini
            var apiKey = _config["Gemini:ApiKey"];
            string rawResponse;
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                var mock = $"[MOCK RESPONSE] Received: {content?.Trim().Substring(0, Math.Min(200, content?.Length ?? 0))}";
                rawResponse = JsonSerializer.Serialize(new
                {
                    candidates = new[]
                    {
                new
                {
                    content = new
                    {
                        parts = new[] { new { text = mock } }
                    }
                }
            }
                });
            }
            else
            {
                var latestUserMessage = new Message
                {
                    Content = content,
                    ImageBase64 = imageBase64,
                    SenderId = senderId
                };

                var payload = BuildGeminiPayload(recent, latestUserMessage);
                var endpoint = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key={apiKey}";
                var client = _httpFactory.CreateClient();
                client.Timeout = TimeSpan.FromSeconds(_timeoutSeconds);
                var json = JsonSerializer.Serialize(payload);

                using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
                cts.CancelAfter(TimeSpan.FromSeconds(_timeoutSeconds));

                var resp = await client.PostAsync(endpoint, new StringContent(json, Encoding.UTF8, "application/json"), cts.Token);
                rawResponse = await resp.Content.ReadAsStringAsync(cts.Token);
            }

            // 3. Parse kết quả từ AI
            var assistantText = ParseTextFromGemini(rawResponse)
                ?? "Không đủ thông tin để định giá";

            // 4. Lưu tin nhắn AI
            var aiMessage = new Message
            {
                Content = assistantText,
                CreatedAt = DateTime.UtcNow,
                ImageBase64 = null,
                AttachmentUrl = null,
                MessageType = MessageType.Text,
                SenderId = receiverId,
                ReceiverId = senderId,
                Status = MessageStatus.Sent
            };
            await _messageService.AddAsync(aiMessage, ct);

            // 5. Gửi duy nhất một tin nhắn hoàn chỉnh cho client
            var groupName = GetGroupName(senderId, receiverId);
            await _hub.Clients.Group(groupName).SendAsync("ReceiveMessage", ToDto(aiMessage), ct);

            return aiMessage;
        }


        private object BuildGeminiPayload(List<Message> recent, Message latestUser)
        {
            var parts = new List<object>();

            var systemPrompt = _config["Chat:SystemPrompt"] ??
@"Bạn là chuyên gia định giá xe máy cũ tại Việt Nam với nhiều năm kinh nghiệm và làm việc cho công ty Horizon Convergia
Dựa vào hình ảnh và mô tả sau đây, bạn hãy đưa ra mức giá hợp lý nhất cho chiếc xe trên thị trường hiện tại tại Việt Nam. 
Nếu mô tả không đầy đủ hoặc không có, bạn hãy ưu tiên dựa vào hình ảnh để ước tính giá.
Chỉ đưa ra một con số giá ước tính tính theo đơn vị VNĐ, giải thích ngắn gọn tại sao lại có giá đó 
Nếu thông tin không đủ rõ hoặc không thể định giá, hãy trả lời ""Không đủ thông tin để định giá""
Thông tin xe:";

            parts.Add(new { text = systemPrompt });

            foreach (var m in recent.OrderBy(m => m.CreatedAt))
            {
                var role = m.SenderId.Equals(_aiBotId, StringComparison.OrdinalIgnoreCase)
                    ? "assistant"
                    : "user";

                if (!string.IsNullOrWhiteSpace(m.Content))
                {
                    parts.Add(new { text = $"{role}: {m.Content}" });
                }

                if (!string.IsNullOrEmpty(m.ImageBase64))
                {
                    parts.Add(new
                    {
                        inline_data = new
                        {
                            mime_type = "image/jpeg",
                            data = m.ImageBase64
                        }
                    });
                }
            }

            if (!string.IsNullOrWhiteSpace(latestUser.Content))
                parts.Add(new { text = $"user: {latestUser.Content}" });
            if (!string.IsNullOrEmpty(latestUser.ImageBase64))
            {
                parts.Add(new
                {
                    inline_data = new
                    {
                        mime_type = "image/jpeg",
                        data = latestUser.ImageBase64
                    }
                });
            }

            return new { contents = new[] { new { parts = parts.ToArray() } } };
        }

        private static IEnumerable<string> ChunkString(string s, int size)
        {
            if (string.IsNullOrEmpty(s)) yield break;
            for (int i = 0; i < s.Length; i += size)
                yield return s.Substring(i, Math.Min(size, s.Length - i));
        }

        private static string? ParseTextFromGemini(string raw)
        {
            try
            {
                using var doc = JsonDocument.Parse(raw);
                var candidates = doc.RootElement.GetProperty("candidates");
                var first = candidates[0];
                var content = first.GetProperty("content");
                var parts = content.GetProperty("parts");
                var firstPart = parts[0];
                var text = firstPart.GetProperty("text").GetString();
                return text;
            }
            catch
            {
                try
                {
                    using var doc = JsonDocument.Parse(raw);
                    if (doc.RootElement.TryGetProperty("candidates", out var c) &&
                        c.GetArrayLength() > 0)
                    {
                        var maybe = c[0].GetProperty("content").GetProperty("parts")[0].GetProperty("text").GetString();
                        return maybe;
                    }
                }
                catch { }
                return null;
            }
        }

        private static object ToDto(Message m) => new
        {
            Id = m.Id,
            Content = m.Content,
            SenderId = m.SenderId,
            ReceiverId = m.ReceiverId,
            MessageType = m.MessageType.ToString(),
            CreatedAt = m.CreatedAt,
            AttachmentUrl = m.AttachmentUrl
        };

        private static string GetGroupName(string a, string b)
        {
            var arr = new[] { a, b }.OrderBy(x => x).ToArray();
            return $"session_{arr[0]}_{arr[1]}";
        }
    }
}
