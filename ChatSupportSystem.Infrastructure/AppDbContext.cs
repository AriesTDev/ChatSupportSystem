using ChatSupportSystem.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatSupportSystem.Infrastructure.Repositories
{
    public class AppDbContext : DbContext
    {
        public DbSet<ChatSession> ChatSessions { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ChatSession>()
                .HasKey(x => x.Id);
        }
    }
}
