using ChatSupportSystem.Shared.Enums;
using System;

namespace ChatSupportSystem.Shared.Models
{
    public class Agent
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Seniority Seniority { get; set; }
        public int MaxConcurrency => 10;
        public double Efficiency => Seniority switch
        {
            Seniority.Junior => 0.4,
            Seniority.MidLevel => 0.6,
            Seniority.Senior => 0.8,
            Seniority.TeamLead => 0.5,
            _ => 0.0
        };
        public bool IsShiftOver { get; set; } // Indicates if the agent's shift is over
        public int CurrentChats { get; set; } // Tracks the number of chats currently assigned
        public int RemainingCapacity => (int)(MaxConcurrency * Efficiency) - CurrentChats;
    }
}
