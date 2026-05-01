using DetectiveInterrogation.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DetectiveInterrogation.Data.Configurations;

public class SuspectConfiguration : IEntityTypeConfiguration<Suspect>
{
    public void Configure(EntityTypeBuilder<Suspect> builder)
    {
        builder.HasKey(s => s.Id);

        builder.ToTable(t =>
        {
            t.HasCheckConstraint(
                "CK_Suspects_InitialTrust_Range",
                "[InitialTrust] BETWEEN 0 AND 100");

            t.HasCheckConstraint(
                "CK_Suspects_InitialAggression_Range",
                "[InitialAggression] BETWEEN 0 AND 100");
        });

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

        builder.Property(s => s.IsGuilty)
            .IsRequired();

        builder.HasOne(s => s.Case)
            .WithMany(c => c.Suspects)
            .HasForeignKey(s => s.CaseId)
            .OnDelete(DeleteBehavior.Cascade);

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
