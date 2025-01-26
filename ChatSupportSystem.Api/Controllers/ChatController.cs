using ChatSupportSystem.Shared.Models.DTOs;
using ChatSupportSystem.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using ChatSupportSystem.Infrastructure.Services;

namespace ChatSupportSystem.Api.Controllers
{
    [ApiController]
    [Route("api/chat")]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpPost("initiate")]
        public async Task<IActionResult> InitiateChat([FromBody] ChatSessionDto chatSessionDto)
        {
            var sessionId = await _chatService.InitiateChatAsync(chatSessionDto);
            return Ok(sessionId);
        }

        [HttpPost("poll")]
        public async Task<IActionResult> PollSession([FromBody] string sessionId)
        {
            var result = await _chatService.PollSessionAsync(Guid.Parse(sessionId));
            return Ok(result);
        }
    }
}
