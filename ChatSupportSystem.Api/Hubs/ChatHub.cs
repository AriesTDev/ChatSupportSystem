using Microsoft.AspNetCore.SignalR;

namespace ChatSupportSystem.Api.Hubs
{
    public class ChatHub : Hub
    {
        // Join a session (add the connection to a group)
        public async Task JoinSession(string sessionId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, sessionId);
        }

        // Leave a session (remove the connection from the group)
        public async Task LeaveSession(string sessionId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, sessionId);
        }

        // Send message to a specific session
        public async Task SendMessageToSession(string sessionId, string message)
        {
            await Clients.Group(sessionId).SendAsync("ReceiveMessage", message);
        }

        // Register Agent
        public async Task RegisterAgent(string message)
        {
            await Clients.Group("QueueMonitor").SendAsync("RegisterAgent", message);
        }
    }
}
