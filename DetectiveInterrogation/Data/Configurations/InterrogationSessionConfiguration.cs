using DetectiveInterrogation.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DetectiveInterrogation.Data.Configurations;

public class InterrogationSessionConfiguration : IEntityTypeConfiguration<InterrogationSession>
{
    public void Configure(EntityTypeBuilder<InterrogationSession> builder)
    {
        builder.HasKey(s => s.Id);

        builder.ToTable(t =>
        {
            t.HasCheckConstraint(
                "CK_InterrogationSessions_CurrentTrust_Range",
                "[CurrentTrust] BETWEEN 0 AND 100");

            t.HasCheckConstraint(
                "CK_InterrogationSessions_CurrentAggression_Range",
                "[CurrentAggression] BETWEEN 0 AND 100");
        });

        builder.Property(s => s.CaseId)
            .IsRequired();

        builder.Property(s => s.SuspectId)
            .IsRequired();

        builder.Property(s => s.UserId)
            .IsRequired();

        builder.Property(s => s.CurrentTrust)
            .IsRequired()
            .HasDefaultValue(50);

        builder.Property(s => s.CurrentAggression)
            .IsRequired()
            .HasDefaultValue(50);

        builder.Property(s => s.Status)
            .IsRequired()
            .HasDefaultValue("InProgress")
            .HasMaxLength(20);

        builder.HasOne(s => s.Case)
            .WithMany()
            .HasForeignKey(s => s.CaseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.Suspect)
            .WithMany(s => s.InterrogationSessions)
            .HasForeignKey(s => s.SuspectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.User)
            .WithMany(u => u.InterrogationSessions)
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.SessionUsedEvidences)
            .WithOne(sue => sue.Session)
            .HasForeignKey(sue => sue.SessionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
