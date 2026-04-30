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
        
        builder.Property(sr => sr.PhraseId)
            .IsRequired();
        
        builder.Property(sr => sr.ReplyText)
            .IsRequired();
        
        builder.Property(sr => sr.TrustChange)
            .HasDefaultValue(0);
        
        builder.Property(sr => sr.AggressionChange)
            .HasDefaultValue(0);
    }
}