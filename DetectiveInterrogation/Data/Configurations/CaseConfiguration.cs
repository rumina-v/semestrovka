using DetectiveInterrogation.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DetectiveInterrogation.Data.Configurations;

public class CaseConfiguration : IEntityTypeConfiguration<Case>
{
    public void Configure(EntityTypeBuilder<Case> builder)
    {
        builder.HasKey(c => c.Id);
        
        builder.Property(c => c.Title)
            .IsRequired()
            .HasMaxLength(255);
        
        builder.Property(c => c.NewspaperText)
            .IsRequired(false);
        
        builder.Property(c => c.ShortDescription)
            .IsRequired(false);
        
        builder.Property(c => c.FullDescription)
            .IsRequired(false);
        
        builder.HasMany(c => c.Suspects)
            .WithOne(s => s.Case)
            .HasForeignKey(s => s.CaseId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(c => c.Evidence)
            .WithOne(e => e.Case)
            .HasForeignKey(e => e.CaseId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(c => c.InterrogationSessions)
            .WithOne(s => s.Case)
            .HasForeignKey(s => s.CaseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}