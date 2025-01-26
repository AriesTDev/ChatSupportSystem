using ChatSupportSystem.Shared.Enums;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatSupportSystem.Shared.Models
{
    public class ChatSession
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public int PollCounter { get; set; }
        public ChatSessionStatus Status { get; set; }
        [NotMapped]
        public Agent? AssignedAgent { get; set; }
    }
}
