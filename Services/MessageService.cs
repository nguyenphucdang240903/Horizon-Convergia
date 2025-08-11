using BusinessObjects.Models;
using DataAccessObjects.Data;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class MessageService : IMessageService
    {
        private readonly AppDbContext _db;
        public MessageService(AppDbContext db) { _db = db; }

        public async Task<Message> AddAsync(Message m, CancellationToken ct = default)
        {
            _db.Messages.Add(m);
            await _db.SaveChangesAsync(ct);
            return m;
        }

        public async Task<IEnumerable<Message>> GetSessionHistoryByParticipantsAsync(string participantA, string participantB, int limit = 100, CancellationToken ct = default)
        {
            return await _db.Messages
                .Where(x => (x.SenderId == participantA && x.ReceiverId == participantB) || (x.SenderId == participantB && x.ReceiverId == participantA))
                .OrderByDescending(x => x.CreatedAt)
                .Take(limit)
                .OrderBy(x => x.CreatedAt)
                .ToListAsync(ct);
        }

        public Task<IEnumerable<Message>> GetHistoryAsync(string userAId, string userBId, int limit = 100, CancellationToken ct = default)
            => GetSessionHistoryByParticipantsAsync(userAId, userBId, limit, ct);
    }

}
