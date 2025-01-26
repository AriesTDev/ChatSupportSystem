using ChatSupportSystem.Api.Hubs;
using ChatSupportSystem.Infrastructure.Repositories;
using ChatSupportSystem.Infrastructure.Services;
using ChatSupportSystem.Shared.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlite("Data Source=D:\\Aries\\AriesTDev\\ChatSupportSystem\\Data\\chatapp.db", options => options.MigrationsAssembly("ChatSupportSystem.Api")));

builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IChatSessionRepository, ChatSessionRepository>();

// Add SignalR service
builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin() // Use AllowSpecificOrigins for production
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseCors();
// Map SignalR Hub
app.MapHub<ChatHub>("/chatHub");

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();
app.Run();