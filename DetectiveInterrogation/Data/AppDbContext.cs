using DetectiveInterrogation.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace DetectiveInterrogation.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Case> Cases { get; set; }
    public DbSet<Suspect> Suspects { get; set; }
    public DbSet<Evidence> Evidence { get; set; }
    public DbSet<EvidencePhrase> EvidencePhrases { get; set; }
    public DbSet<SuspectReply> SuspectReplies { get; set; }
    public DbSet<InterrogationSession> InterrogationSessions { get; set; }
    public DbSet<SessionUsedEvidence> SessionUsedEvidences { get; set; }
    public DbSet<Achievement> Achievements { get; set; }
    public DbSet<UserAchievement> UserAchievements { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations
        modelBuilder.ApplyConfiguration(new Configurations.UserConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.CaseConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.SuspectConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.EvidenceConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.EvidencePhraseConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.SuspectReplyConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.InterrogationSessionConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.SessionUsedEvidenceConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.AchievementConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.UserAchievementConfiguration());
    }
}
