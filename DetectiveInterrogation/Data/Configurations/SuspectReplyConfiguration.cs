using DetectiveInterrogation.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DetectiveInterrogation.Data.Configurations;

public class SuspectReplyConfiguration : IEntityTypeConfiguration<SuspectReply>
{
    public void Configure(EntityTypeBuilder<SuspectReply> builder)
    {
        builder.HasKey(sr => sr.Id);
        
        builder.Property(sr => sr.SuspectId)
            .IsRequired();
        
        builder.Property(sr => sr.EvidencePhraseId)
            .IsRequired();
        
        builder.Property(sr => sr.MinTrust)
            .IsRequired();

        builder.Property(sr => sr.MaxTrust)
            .IsRequired();

        builder.Property(sr => sr.MinPressure)
            .IsRequired();

        builder.Property(sr => sr.MaxPressure)
            .IsRequired();

        builder.Property(sr => sr.Text)
            .IsRequired();
    }
}
