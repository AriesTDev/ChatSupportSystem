using System.Collections.Generic;
using System.Linq;

namespace ChatSupportSystem.Shared.Models
{
    public class Team
    {
        public string Name { get; set; }
        public List<Agent> Agents { get; set; } = new List<Agent>();
        public int Capacity => Agents.Sum(a => (int)(a.MaxConcurrency * a.Efficiency));
        public int MaxQueueLength => (int)(Capacity * 1.5);
    }
}
