using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;


    public class ChatHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            // Optionally auto-join group if query contains participantA & participantB
            var http = Context.GetHttpContext();
            if (http != null)
            {
                var pA = http.Request.Query["participantA"].FirstOrDefault();
                var pB = http.Request.Query["participantB"].FirstOrDefault();
                if (!string.IsNullOrEmpty(pA) && !string.IsNullOrEmpty(pB))
                {
                    var arr = new[] { pA, pB }.OrderBy(x => x).ToArray();
                    var group = $"session_{arr[0]}_{arr[1]}";
                    await Groups.AddToGroupAsync(Context.ConnectionId, group);
                }
            }
            await base.OnConnectedAsync();
        }

        public Task JoinSession(string participantA, string participantB)
        {
            var arr = new[] { participantA, participantB }.OrderBy(x => x).ToArray();
            var group = $"session_{arr[0]}_{arr[1]}";
            return Groups.AddToGroupAsync(Context.ConnectionId, group);
        }

        public Task LeaveSession(string participantA, string participantB)
        {
            var arr = new[] { participantA, participantB }.OrderBy(x => x).ToArray();
            var group = $"session_{arr[0]}_{arr[1]}";
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, group);
        }
    public async Task SendMessage(string senderId, string receiverId, string content)
    {
        // Lưu DB hoặc gọi service xử lý ở đây nếu cần
        var arr = new[] { senderId, receiverId }.OrderBy(x => x).ToArray();
        var group = $"session_{arr[0]}_{arr[1]}";

        var message = new
        {
            Id = Guid.NewGuid().ToString(),
            Content = content,
            SenderId = senderId,
            ReceiverId = receiverId,
            CreatedAt = DateTime.UtcNow
        };

        // Phát tin nhắn tới tất cả client trong group
        await Clients.Group(group).SendAsync("ReceiveMessage", message);
    }
}

