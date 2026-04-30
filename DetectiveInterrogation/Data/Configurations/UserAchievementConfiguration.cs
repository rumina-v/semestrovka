using DetectiveInterrogation.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DetectiveInterrogation.Data.Configurations;

public class UserAchievementConfiguration : IEntityTypeConfiguration<UserAchievement>
{
    public void Configure(EntityTypeBuilder<UserAchievement> builder)
    {
        builder.HasKey(ua => ua.Id);
        
        builder.Property(ua => ua.UserId)
            .IsRequired();
        
        builder.Property(ua => ua.AchievementId)
            .IsRequired();
        
        // Composite unique constraint to prevent duplicate achievements
        builder.HasIndex(ua => new { ua.UserId, ua.AchievementId }).IsUnique();
    }
}