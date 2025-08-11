using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Services.Interfaces;

namespace HorizonConvergia.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chat;
        private readonly IMessageService _msg;
        private readonly IHubContext<ChatHub> _hub;

        public ChatController(
            IChatService chat,
            IMessageService msg,
            IHubContext<ChatHub> hub
        )
        {
            _chat = chat;
            _msg = msg;
            _hub = hub;
        }

        [HttpPost("send")]
        [RequestSizeLimit(25_000_000)]
        public async Task<IActionResult> Send(
            [FromForm] string senderId,
            [FromForm] string receiverId,
            [FromForm] string content,
            IFormFile? image
        )
        {
            // Gọi ChatService (nếu là AI thì ChatService sẽ tự xử lý và stream)
            var userMsg = await _chat.HandleUserMessageAsync(senderId, receiverId, content, image);

            // Trả dữ liệu gọn nhẹ cho client
            return Ok(new
            {
                Content = userMsg.Content
            });
        }

        [HttpGet("history")]
        public async Task<IActionResult> History(
            [FromQuery] string a,
            [FromQuery] string b,
            [FromQuery] int limit = 200
        )
        {
            var list = await _msg.GetSessionHistoryByParticipantsAsync(a, b, limit);
            var dto = list.Select(m => new
            {
                m.Id,
                m.Content,
                m.SenderId,
                m.ReceiverId,
                m.CreatedAt,
                m.AttachmentUrl,
                MessageType = m.MessageType.ToString()
            });
            return Ok(dto);
        }
    }
}
