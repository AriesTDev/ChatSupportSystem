using ChatSupportSystem.Infrastructure.Repositories;
using ChatSupportSystem.Shared.Enums;
using ChatSupportSystem.Shared.Models;
using ChatSupportSystem.Shared.Models.DTOs;
using System;
using System.Threading.Tasks;

namespace ChatSupportSystem.Infrastructure.Services
{
    public class ChatService : IChatService
    {
        private readonly IChatSessionRepository _repository;

        public ChatService(IChatSessionRepository repository)
        {
            _repository = repository;
        }

        public async Task<Guid> InitiateChatAsync(ChatSessionDto chatSessionDto)
        {
            var chatSession = new ChatSession
            {
                Id = Guid.NewGuid(),
                UserId = chatSessionDto.UserId,
                CreatedAt = DateTime.UtcNow,
                PollCounter = 0,
                Status = ChatSessionStatus.Queued,
            };

            await _repository.AddAsync(chatSession);
            return chatSession.Id;
        }

        public async Task<bool> PollSessionAsync(Guid sessionId)
        {
            var session = await _repository.GetByIdAsync(sessionId);
            if (session == null || session.Status != ChatSessionStatus.Active || session.PollCounter >= 3)
            {
                return false;
            }

            session.PollCounter = session.PollCounter + 1;
            await _repository.UpdateAsync(session);
            return session.Status == ChatSessionStatus.Active;
        }
    }
}
