using BusinessObjects.Models;
using DataAccessObjects.Data;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly AppDbContext _db;
        public MessageRepository(AppDbContext db) { _db = db; }

        public async Task<Message> AddAsync(Message message)
        {
            if (string.IsNullOrEmpty(message.Id)) message.Id = Guid.NewGuid().ToString();
            message.CreatedAt = DateTime.UtcNow;
            _db.Messages.Add(message);
            await _db.SaveChangesAsync();
            return message;
        }

        public async Task<Message> GetByIdAsync(string id)
        {
            return await _db.Messages.FindAsync(id);
        }

        public async Task<IEnumerable<Message>> GetConversationMessagesAsync(string userAId, string userBId, int limit = 50, int offset = 0)
        {
            return await _db.Messages
                .Where(m =>
                    (m.SenderId == userAId && m.ReceiverId == userBId) ||
                    (m.SenderId == userBId && m.ReceiverId == userAId))
                .OrderByDescending(m => m.CreatedAt)
                .Skip(offset)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<IEnumerable<Message>> GetUserMessagesAsync(string userId, int limit = 50)
        {
            return await _db.Messages
                .Where(m => m.SenderId == userId || m.ReceiverId == userId)
                .OrderByDescending(m => m.CreatedAt)
                .Take(limit)
                .ToListAsync();
        }
    }

}
