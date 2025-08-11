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
        Task<Message> AddAsync(Message m, CancellationToken ct = default);
        Task<IEnumerable<Message>> GetHistoryAsync(string userAId, string userBId, int limit = 100, CancellationToken ct = default);
        Task<IEnumerable<Message>> GetSessionHistoryByParticipantsAsync(string participantA, string participantB, int limit = 100, CancellationToken ct = default);
    }

}
