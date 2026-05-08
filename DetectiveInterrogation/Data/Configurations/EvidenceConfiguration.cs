using DetectiveInterrogation.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DetectiveInterrogation.Data.Configurations;

public class EvidenceConfiguration : IEntityTypeConfiguration<Evidence>
{
    public void Configure(EntityTypeBuilder<Evidence> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.CaseId)
            .IsRequired();

        builder.Property(e => e.SuspectId)
            .IsRequired()
            .HasMaxLength(20);
        
        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(255);
        
        builder.Property(e => e.Description)
            .IsRequired();
        
        builder.HasMany(e => e.Phrases)
            .WithOne(p => p.Evidence)
            .HasForeignKey(p => p.EvidenceId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(e => e.SessionUsedEvidences)
            .WithOne(sue => sue.Evidence)
            .HasForeignKey(sue => sue.EvidenceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
