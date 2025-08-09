using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace HorizonConvergia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // bắt buộc login
    public class ChatController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public ChatController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        // gửi tin nhắn P2P
        [HttpPost("send")]
        public async Task<IActionResult> Send([FromBody] SendMessageRequest req)
        {
            // req.SenderId có thể lấy từ claim thay vì client gửi
            var senderId = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (string.IsNullOrEmpty(senderId)) return Unauthorized();

            var msg = await _messageService.SendPrivateMessageAsync(senderId, req.ReceiverId, req.Content);
            return Ok(msg);
        }

        [HttpGet("history/{otherUserId}")]
        public async Task<IActionResult> GetHistory(string otherUserId, int limit = 50, int offset = 0)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var msgs = await _messageService.GetConversationAsync(userId, otherUserId, limit, offset);
            return Ok(msgs);
        }
    }

    public class SendMessageRequest
    {
        public string ReceiverId { get; set; }
        public string Content { get; set; }
    }
}
