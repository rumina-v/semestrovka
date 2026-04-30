using DetectiveInterrogation.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DetectiveInterrogation.Data.Configurations;

public class SessionUsedEvidenceConfiguration : IEntityTypeConfiguration<SessionUsedEvidence>
{
    public void Configure(EntityTypeBuilder<SessionUsedEvidence> builder)
    {
        builder.HasKey(sue => sue.Id);
        
        builder.Property(sue => sue.EvidenceId)
            .IsRequired();
        
        builder.Property(sue => sue.SessionId)
            .IsRequired();
        
        // Composite unique constraint to prevent duplicate evidence usage in session
        builder.HasIndex(sue => new { sue.SessionId, sue.EvidenceId }).IsUnique();
    }
}