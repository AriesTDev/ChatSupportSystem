using ChatSupportSystem.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using QueueMonitor.Service;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlite("D:\\Aries\\AriesTDev\\ChatSupportSystem\\Data\\chatapp.db"));

builder.Services.AddScoped<IChatSessionRepository, ChatSessionRepository>();
builder.Services.AddHostedService<QueueMonitorService>();

var host = builder.Build();
host.Run();
