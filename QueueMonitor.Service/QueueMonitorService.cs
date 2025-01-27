using ChatSupportSystem.Infrastructure.Repositories;
using ChatSupportSystem.Shared.Enums;
using ChatSupportSystem.Shared.Models;
using Microsoft.AspNetCore.SignalR.Client;
using System.Text.Json;

namespace QueueMonitor.Service
{
    public class QueueMonitorService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<QueueMonitorService> _logger;
        private readonly IConfiguration _configuration;

        private HubConnection _hubConnection;
        private List<Agent> _agents;

        private readonly int _maxPollCount = 3;
        private readonly int _maxConcurrency = 10;
        private readonly double _overflowMultiplier = 1.5;

        private readonly TimeSpan _officeStartTime = new TimeSpan(8, 0, 0); // 8:00 AM
        private readonly TimeSpan _officeEndTime = new TimeSpan(17, 0, 0); // 5:00 PM

        public QueueMonitorService(ILogger<QueueMonitorService> logger,
                                   IConfiguration configuration,
                                   IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _configuration = configuration;
            //_agents = new();
            _agents = new List<Agent>
            {
                new Agent { Id = Guid.NewGuid(), Name = "SeniorAgent", Seniority = Seniority.Senior, CurrentChats = 0, IsShiftOver = false },
                new Agent { Id = Guid.NewGuid(), Name = "JuniorAgent", Seniority = Seniority.Junior, CurrentChats = 0, IsShiftOver = false }
            };
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
             await InitializeSignalRConnection();

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await MonitorQueueAsync(stoppingToken);
                    await Task.Delay(1000, stoppingToken); // Monitor every second
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred in QueueMonitorService.");
                }
            }
        }

        private async Task InitializeSignalRConnection()
        {
            var hubUrl = _configuration["SignalR:Url"];

            if (string.IsNullOrEmpty(hubUrl))
            {
                _logger.LogError("SignalR URL is not configured in appsettings.");
                return;
            }

            try
            {
                _hubConnection = new HubConnectionBuilder()
                    .WithUrl(hubUrl)
                    .Build();

                _hubConnection.On<string>("RegisterAgent", (message) =>
                {
                    var agent = JsonSerializer.Deserialize<Agent>(message);

                    var existingAgent = _agents.FirstOrDefault(a => a.Name == agent.Name);
                    if (existingAgent == null)
                        _agents.Add(new Agent
                        {
                            Id = Guid.NewGuid(),
                            Name = agent.Name,
                            CurrentChats = 0,
                            IsShiftOver = false,
                            Seniority = agent.Seniority
                        });
                });

                // Connect to the SignalR Hub
                await _hubConnection.StartAsync();

                await _hubConnection.SendAsync("JoinSession", "QueueMonitor"); // Join the session group

                _logger.LogInformation("SignalR connection established.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error connecting to SignalR Hub.");
            }
        }

        private async Task MonitorQueueAsync(CancellationToken cancellationToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var sessionRepository = scope.ServiceProvider.GetRequiredService<IChatSessionRepository>();

                var sessions = await sessionRepository.GetAllQueuedSessionsAsync();

                foreach (var session in sessions)
                {
                    // Mark sessions as inactive if they exceed poll count
                    if (session.PollCounter > _maxPollCount)
                    {
                        session.Status = ChatSessionStatus.Rejected;
                        await sessionRepository.UpdateAsync(session);
                    }
                }

                var updatedSessions = AssignChatsToAgents(_agents, sessions, IsDuringOfficeHours());

                // Update session assigned and publish message to SignalR
                foreach (var session in updatedSessions)
                {
                    await sessionRepository.UpdateAsync(session);

                    if (session.Status == ChatSessionStatus.Active && session.AssignedAgent != null)
                    {
                        await _hubConnection.SendAsync(
                            "SendMessageToSession",
                            session.Id,
                            $"Agent {session.AssignedAgent?.Name} has been assigned."
                        );

                        await _hubConnection.SendAsync(
                            "SendMessageToSession",
                            session.Id,
                            $"Hello {session.UserId}! How may I help you?"
                        );
                    }
                }
            }
        }


        public List<ChatSession> AssignChatsToAgents(List<Agent> agents, List<ChatSession> sessions, bool isDuringOfficeHours)
        {
            HandleOverflow(sessions, agents, isDuringOfficeHours);

            foreach (var session in sessions.Where(s => s.Status == ChatSessionStatus.Queued))
            {
                var availableAgent = agents
                    .Where(a => !a.IsShiftOver && a.RemainingCapacity > 0)
                    .OrderBy(o => (int)o.Seniority)
                    .ThenByDescending(o => o.RemainingCapacity)
                    .FirstOrDefault();

                if (availableAgent != null)
                {
                    session.Status = ChatSessionStatus.Active;
                    session.AssignedAgent = availableAgent;
                    availableAgent.CurrentChats++;
                }
            }

            return sessions;
        }

        public void HandleOverflow(List<ChatSession> sessions, List<Agent> agents, bool isDuringOfficeHours)
        {
            // Calculate current team capacity
            int teamCapacity = agents.Where(a => !a.IsShiftOver).Sum(a => a.RemainingCapacity);
            int maxQueueLength = (int)(teamCapacity * _overflowMultiplier);

            // If queue exceeds maxQueueLength during office hours, activate overflow
            int queuedSessions = sessions.Count(s => s.Status == ChatSessionStatus.Queued);
            if (queuedSessions > maxQueueLength && IsDuringOfficeHours())
            {
                int overflowTeamCount = (int)Math.Ceiling((queuedSessions - maxQueueLength) / 4.0m); //junior agent capacity is 4

                for (int i = 0; i < overflowTeamCount; i++)
                {
                    var overflowAgent = new Agent
                    {
                        Id = Guid.NewGuid(),
                        Name = $"OverflowAgent_{i + 1}",
                        Seniority = Seniority.Junior,
                        CurrentChats = 0,
                        IsShiftOver = false
                    };
                    agents.Add(overflowAgent);

                    _logger.LogInformation($"Overflow agent {overflowAgent.Name} added to the team.");
                }

            }
        }

        private bool IsDuringOfficeHours()
        {
            return true;

            //var currentTime = DateTime.Now.TimeOfDay;
            //return currentTime >= _officeStartTime && currentTime <= _officeEndTime;
        }

    }
}
