using DetectiveInterrogation.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DetectiveInterrogation.Data.Configurations;

public class EvidencePhraseConfiguration : IEntityTypeConfiguration<EvidencePhrase>
{
    public void Configure(EntityTypeBuilder<EvidencePhrase> builder)
    {
        builder.HasKey(ep => ep.Id);
        
        builder.Property(ep => ep.EvidenceId)
            .IsRequired();
        
        builder.Property(ep => ep.Text)
            .IsRequired();
        
        builder.HasMany(ep => ep.SuspectReplies)
            .WithOne(sr => sr.Phrase)
            .HasForeignKey(sr => sr.PhraseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}