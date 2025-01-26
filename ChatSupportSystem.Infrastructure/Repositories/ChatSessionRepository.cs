using ChatSupportSystem.Shared.Enums;
using ChatSupportSystem.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatSupportSystem.Infrastructure.Repositories
{
    public class ChatSessionRepository : IChatSessionRepository
    {
        private readonly AppDbContext _context;

        public ChatSessionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ChatSession chatSession)
        {
            await _context.ChatSessions.AddAsync(chatSession);
            await _context.SaveChangesAsync();
        }

        public async Task<ChatSession> GetByIdAsync(Guid id)
        {
            return await _context.ChatSessions.FindAsync(id);
        }

        public async Task<List<ChatSession>> GetAllQueuedSessionsAsync()
        {
            return await _context.ChatSessions
                .Where(x => x.Status == ChatSessionStatus.Queued)
                .ToListAsync();
        }

        public async Task UpdateAsync(ChatSession chatSession)
        {
            _context.ChatSessions.Update(chatSession);
            await _context.SaveChangesAsync();
        }
    }
}
