using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IMessageRepository
    {
        Task<Message> AddAsync(Message message);
        Task<IEnumerable<Message>> GetConversationMessagesAsync(string userAId, string userBId, int limit = 50, int offset = 0);
        Task<IEnumerable<Message>> GetUserMessagesAsync(string userId, int limit = 50);
        Task<Message> GetByIdAsync(string id);
    }
}
