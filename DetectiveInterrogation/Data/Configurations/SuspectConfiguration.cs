using DetectiveInterrogation.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DetectiveInterrogation.Data.Configurations;

public class SuspectConfiguration : IEntityTypeConfiguration<Suspect>
{
    public void Configure(EntityTypeBuilder<Suspect> builder)
    {
        builder.HasKey(s => s.Id);
        
        builder.Property(s => s.CaseId)
            .IsRequired();
        
        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(255);
        
        builder.Property(s => s.Description)
            .IsRequired(false);
        
        builder.Property(s => s.InitialTrust)
            .IsRequired()
            .HasDefaultValue(50);
        
        builder.Property(s => s.InitialAggression)
            .IsRequired()
            .HasDefaultValue(50);
        
        // Check constraints for trust and aggression
        builder.Property(s => s.InitialTrust).HasConversion(new Microsoft.EntityFrameworkCore.Storage.ValueConverter<int, int>(
            v => v > 100 ? 100 : (v < 0 ? 0 : v),
            v => v));
        
        builder.Property(s => s.InitialAggression).HasConversion(new Microsoft.EntityFrameworkCore.Storage.ValueConverter<int, int>(
            v => v > 100 ? 100 : (v < 0 ? 0 : v),
            v => v));
        
        builder.HasMany(s => s.Replies)
            .WithOne(r => r.Suspect)
            .HasForeignKey(r => r.SuspectId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(s => s.InterrogationSessions)
            .WithOne(s => s.Suspect)
            .HasForeignKey(s => s.SuspectId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}