using Microsoft.EntityFrameworkCore;
using WayfinderBackend.Api.Models;

namespace WayfinderBackend.Api.Data
{
    public class WayfinderDbContext : DbContext
    {
        public WayfinderDbContext(DbContextOptions<WayfinderDbContext> options)
            : base(options)
        {
        }

        public DbSet<NavigationSession> NavigationSessions { get; set; }
        public DbSet<NavigationStep> NavigationSteps { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NavigationSession>(entity =>
            {
                entity.HasKey(e => e.SessionId);
                entity.Property(e => e.SessionId).ValueGeneratedNever();
                entity.HasMany(e => e.Steps)
                      .WithOne()
                      .HasForeignKey("NavigationSessionId")
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<NavigationStep>(entity =>
            {
                entity.HasKey(e => new { e.NavigationSessionId, e.Order });
                entity.Property(e => e.NavigationSessionId).IsRequired();
                entity.Property(e => e.Order).IsRequired();
                entity.Property(e => e.Instruction).IsRequired();
                entity.Property(e => e.Direction).IsRequired();
            });
        }
    }
}
