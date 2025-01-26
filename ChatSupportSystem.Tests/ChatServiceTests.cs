
using ChatSupportSystem.Infrastructure.Repositories;
using ChatSupportSystem.Shared.Models;
using ChatSupportSystem.Shared.Models.DTOs;
using Moq;

namespace ChatSupportSystem.Tests
{
    public class ChatServiceTests
    {
        private readonly Mock<IChatSessionRepository> _mockRepository;
        private readonly IChatService _chatService;

        public ChatServiceTests()
        {
            _mockRepository = new Mock<IChatSessionRepository>();
            _chatService = new Infrastructure.Services.ChatService(_mockRepository.Object);
        }

        [Fact]
        public async Task InitiateChatAsync_ShouldReturnGuid()
        {
            var chatDto = new ChatSessionDto { UserId = "test-user" };

            var result = await _chatService.InitiateChatAsync(chatDto);

            Assert.NotEqual(Guid.Empty, result);
        }
    }
}