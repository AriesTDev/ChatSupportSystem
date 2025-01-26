using ChatSupportSystem.Shared.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatSupportSystem.Infrastructure.Repositories
{
    public interface IChatSessionRepository
    {
        Task AddAsync(ChatSession chatSession);
        Task<ChatSession> GetByIdAsync(Guid id);
        Task<List<ChatSession>> GetAllQueuedSessionsAsync();
        Task UpdateAsync(ChatSession chatSession);
    }
}
