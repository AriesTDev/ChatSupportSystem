
using ChatSupportSystem.Infrastructure.Repositories;
using ChatSupportSystem.Shared.Enums;
using ChatSupportSystem.Shared.Models;
using ChatSupportSystem.Shared.Models.DTOs;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using QueueMonitor.Service;

namespace ChatSupportSystem.Tests
{
    public class QueueMonitorServiceTests
    {
        private readonly Mock<IChatSessionRepository> _mockSessionRepository;
        private readonly Mock<ILogger<QueueMonitorService>> _mockLogger;
        private readonly Mock<IServiceProvider> _mockServiceProvider;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<HubConnection> _mockHubConnection;

        public QueueMonitorServiceTests()
        {
            _mockSessionRepository = new Mock<IChatSessionRepository>();
            _mockLogger = new Mock<ILogger<QueueMonitorService>>();
            _mockServiceProvider = new Mock<IServiceProvider>();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockHubConnection = new Mock<HubConnection>(MockBehavior.Loose);
        }

        [Fact]
        public async Task AssignChats_TeamOfOneSnrOneJnr_ShouldAssignCorrectly()
        {
            // Arrange
            var agents = new List<Agent>
            {
                new Agent { Id = Guid.NewGuid(), Name = "SeniorAgent", Seniority = Seniority.Senior, CurrentChats = 0, IsShiftOver = false },
                new Agent { Id = Guid.NewGuid(), Name = "JuniorAgent", Seniority = Seniority.Junior, CurrentChats = 0, IsShiftOver = false }
            };

            var sessions = new List<ChatSession>
            {
                new ChatSession { Id = Guid.NewGuid(), Status = ChatSessionStatus.Queued, PollCounter = 0, UserId = "User1" },
                new ChatSession { Id = Guid.NewGuid(), Status = ChatSessionStatus.Queued, PollCounter = 0, UserId = "User2" },
                new ChatSession { Id = Guid.NewGuid(), Status = ChatSessionStatus.Queued, PollCounter = 0, UserId = "User3" },
                new ChatSession { Id = Guid.NewGuid(), Status = ChatSessionStatus.Queued, PollCounter = 0, UserId = "User4" },
                new ChatSession { Id = Guid.NewGuid(), Status = ChatSessionStatus.Queued, PollCounter = 0, UserId = "User5" }
            };

            _mockSessionRepository.Setup(repo => repo.GetAllQueuedSessionsAsync()).ReturnsAsync(sessions);
            _mockServiceProvider.Setup(sp => sp.GetService(typeof(IChatSessionRepository))).Returns(_mockSessionRepository.Object);

            var service = new QueueMonitorService(
                _mockLogger.Object,
                _mockConfiguration.Object,
                _mockServiceProvider.Object
            );

            // Act
            var assignedSessions = service.AssignChatsToAgents(agents, sessions);

            // Assert
            var juniorAgent = agents.First(a => a.Name == "JuniorAgent");
            var seniorAgent = agents.First(a => a.Name == "SeniorAgent");
            var juniorAgentSessions = assignedSessions.Where(a => a.AssignedAgent?.Name == "JuniorAgent").ToList();
            var seniorAgentSessions = assignedSessions.Where(a => a.AssignedAgent?.Name == "SeniorAgent").ToList();

            Assert.Equal(4, juniorAgent.CurrentChats);
            Assert.Equal(1, seniorAgent.CurrentChats);
            Assert.Contains(juniorAgentSessions, session => session.UserId == "User1");
            Assert.Contains(juniorAgentSessions, session => session.UserId == "User2");
            Assert.Contains(juniorAgentSessions, session => session.UserId == "User3");
            Assert.Contains(juniorAgentSessions, session => session.UserId == "User4");
            Assert.Contains(seniorAgentSessions, session => session.UserId == "User5");
        }

        [Fact]
        public async Task AssignChats_TeamOfTwoJnrOneMid_ShouldAssignCorrectly()
        {
            // Arrange
            var agents = new List<Agent>
            {
                new Agent { Id = Guid.NewGuid(), Name = "JuniorAgent1", Seniority = Seniority.Junior, CurrentChats = 0, IsShiftOver = false },
                new Agent { Id = Guid.NewGuid(), Name = "JuniorAgent2", Seniority = Seniority.Junior, CurrentChats = 0, IsShiftOver = false },
                new Agent { Id = Guid.NewGuid(), Name = "MidAgent", Seniority = Seniority.MidLevel, CurrentChats = 0, IsShiftOver = false }
            };
    
            var sessions = new List<ChatSession>
            {
                new ChatSession { Id = Guid.NewGuid(), Status = ChatSessionStatus.Queued, PollCounter = 0, UserId = "User1" },
                new ChatSession { Id = Guid.NewGuid(), Status = ChatSessionStatus.Queued, PollCounter = 0, UserId = "User2" },
                new ChatSession { Id = Guid.NewGuid(), Status = ChatSessionStatus.Queued, PollCounter = 0, UserId = "User3" },
                new ChatSession { Id = Guid.NewGuid(), Status = ChatSessionStatus.Queued, PollCounter = 0, UserId = "User4" },
                new ChatSession { Id = Guid.NewGuid(), Status = ChatSessionStatus.Queued, PollCounter = 0, UserId = "User5" },
                new ChatSession { Id = Guid.NewGuid(), Status = ChatSessionStatus.Queued, PollCounter = 0, UserId = "User6" }
            };

            _mockSessionRepository.Setup(repo => repo.GetAllQueuedSessionsAsync()).ReturnsAsync(sessions);
            _mockServiceProvider.Setup(sp => sp.GetService(typeof(IChatSessionRepository))).Returns(_mockSessionRepository.Object);

            var service = new QueueMonitorService(
                _mockLogger.Object,
                _mockConfiguration.Object,
                _mockServiceProvider.Object
            );

            // Act
            var assignedSessions = service.AssignChatsToAgents(agents, sessions);

            // Assert
            var juniorAgent1 = agents.First(a => a.Name == "JuniorAgent1");
            var juniorAgent2 = agents.First(a => a.Name == "JuniorAgent2");
            var midAgent = agents.First(a => a.Name == "MidAgent");

            Assert.Equal(3, juniorAgent1.CurrentChats);
            Assert.Equal(3, juniorAgent2.CurrentChats);
            Assert.Equal(0, midAgent.CurrentChats);
        }
    }
}