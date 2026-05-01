using DetectiveInterrogation.Helpers;
using DetectiveInterrogation.Models.Entities;

namespace DetectiveInterrogation.Data;

public static class DbInitializer
{
    public static void Initialize(AppDbContext context, PasswordHasher passwordHasher)
    {
        if (!context.Achievements.Any())
        {
            var achievements = new Achievement[]
            {
                new Achievement
                {
                    Title = "First Interrogation",
                    Description = "Complete your first interrogation"
                },
                new Achievement
                {
                    Title = "Truth Seeker",
                    Description = "Reach 100 trust with a suspect"
                },
                new Achievement
                {
                    Title = "Pressure Expert",
                    Description = "Reach maximum pressure with a suspect"
                },
                new Achievement
                {
                    Title = "Master Detective",
                    Description = "Solve your first case"
                },
                new Achievement
                {
                    Title = "Perfect Investigation",
                    Description = "Achieve all optimal outcomes"
                }
            };

            context.Achievements.AddRange(achievements);
        }

        if (!context.Users.Any(u => u.Role == "Admin"))
        {
            var admin = new User
            {
                Username = "admin",
                Email = "admin@detective.local",
                PasswordHash = passwordHasher.HashPassword("Admin123!"),
                Role = "Admin"
            };

            context.Users.Add(admin);
        }

        context.SaveChanges();
    }
}
