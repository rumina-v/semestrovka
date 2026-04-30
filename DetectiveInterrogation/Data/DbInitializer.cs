using DetectiveInterrogation.Models.Entities;

namespace DetectiveInterrogation.Data;

public static class DbInitializer
{
    public static void Initialize(AppDbContext context)
    {
        // Ensure database is created
        context.Database.EnsureCreated();

        // If database already has data, don't add more
        if (context.Users.Any())
        {
            return;
        }

        // Add seed data
        var achievements = new Achievement[]
        {
            new Achievement { Title = "First Interrogation", Description = "Complete your first interrogation" },
            new Achievement { Title = "Truth Seeker", Description = "Reach 100 trust with a suspect" },
            new Achievement { Title = "Pressure Expert", Description = "Reach maximum pressure with a suspect" },
            new Achievement { Title = "Master Detective", Description = "Solve your first case" },
            new Achievement { Title = "Perfect Investigation", Description = "Achieve all optimal outcomes" }
        };

        context.Achievements.AddRange(achievements);
        context.SaveChanges();
    }
}
