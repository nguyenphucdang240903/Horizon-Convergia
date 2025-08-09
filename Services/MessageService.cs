using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Models;
using Microsoft.AspNetCore.SignalR;
using Repositories.Interfaces;
using Services.Interfaces;

namespace Services
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _repo;
        private readonly IHubContext<ChatHub> _hub;

        public MessageService(IMessageRepository repo, IHubContext<ChatHub> hub)
        {
            _repo = repo;
            _hub = hub;
        }

        public async Task<Message> SendPrivateMessageAsync(string senderId, string receiverId, string content)
        {
            var message = new Message
            {
                Id = Guid.NewGuid().ToString(),
                Content = content,
                SenderId = senderId,
                ReceiverId = receiverId,
                CreatedAt = DateTime.UtcNow
            };

            var saved = await _repo.AddAsync(message);

            // Gửi realtime tới user group (group "user_{userId}" được add tại OnConnectedAsync)
            await _hub.Clients.Group($"user_{receiverId}")
                .SendAsync("ReceiveMessage", new
                {
                    Id = saved.Id,
                    Content = saved.Content,
                    SenderId = saved.SenderId,
                    ReceiverId = saved.ReceiverId,
                    CreatedAt = saved.CreatedAt
                });

            // Optionally gửi tới sender để confirm deliver
            await _hub.Clients.Group($"user_{senderId}")
                .SendAsync("MessageSent", new
                {
                    Id = saved.Id,
                    Content = saved.Content,
                    ReceiverId = saved.ReceiverId,
                    CreatedAt = saved.CreatedAt
                });

            return saved;
        }

        public async Task<IEnumerable<Message>> GetConversationAsync(string userAId, string userBId, int limit = 50, int offset = 0)
        {
            return await _repo.GetConversationMessagesAsync(userAId, userBId, limit, offset);
        }
    }

}
