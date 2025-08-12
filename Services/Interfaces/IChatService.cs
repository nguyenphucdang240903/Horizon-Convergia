using BusinessObjects.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IChatService
    {
        
        Task<Message> HandleUserMessageAsync(string senderId, string receiverId, string content, IFormFile? image = null, CancellationToken ct = default);
    }
}
