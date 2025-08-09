using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IMessageService
    {
        Task<Message> SendPrivateMessageAsync(string senderId, string receiverId, string content);
        Task<IEnumerable<Message>> GetConversationAsync(string userAId, string userBId, int limit = 50, int offset = 0);
    }

}
