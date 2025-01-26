using ChatSupportSystem.Shared.Models.DTOs;
using System;
using System.Threading.Tasks;

namespace ChatSupportSystem.Shared.Models
{
    public interface IChatService
    {
        Task<Guid> InitiateChatAsync(ChatSessionDto chatSessionDto);
        Task<bool> PollSessionAsync(Guid sessionId);
    }
}
