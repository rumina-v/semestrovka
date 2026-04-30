using DetectiveInterrogation.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DetectiveInterrogation.Data.Configurations;

public class InterrogationSessionConfiguration : IEntityTypeConfiguration<InterrogationSession>
{
    public void Configure(EntityTypeBuilder<InterrogationSession> builder)
    {
        builder.HasKey(s => s.Id);
        
        builder.Property(s => s.CaseId)
            .IsRequired();
        
        builder.Property(s => s.SuspectId)
            .IsRequired();
        
        builder.Property(s => s.UserId)
            .IsRequired();
        
        builder.Property(s => s.CurrentTrust)
            .HasDefaultValue(50);
        
        builder.Property(s => s.CurrentAggression)
            .HasDefaultValue(50);
        
        // Check constraints for trust and aggression
        builder.Property(s => s.CurrentTrust).HasConversion(new Microsoft.EntityFrameworkCore.Storage.ValueConverter<int, int>(
            v => v > 100 ? 100 : (v < 0 ? 0 : v),
            v => v));
        
        builder.Property(s => s.CurrentAggression).HasConversion(new Microsoft.EntityFrameworkCore.Storage.ValueConverter<int, int>(
            v => v > 100 ? 100 : (v < 0 ? 0 : v),
            v => v));
        
        builder.Property(s => s.Status)
            .HasDefaultValue("InProgress")
            .HasMaxLength(20);
        
        builder.HasMany(s => s.SessionUsedEvidences)
            .WithOne(sue => sue.Session)
            .HasForeignKey(sue => sue.SessionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}