using Microsoft.EntityFrameworkCore;
using MentorMenteeApp.API.Data;
using MentorMenteeApp.API.Enums;
using MentorMenteeApp.API.Models.Entities;

namespace MentorMenteeApp.API.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(AppDbContext context)
    {
        // Ensure database is created
        await context.Database.EnsureCreatedAsync();

        // Check if data already exists
        if (await context.Users.AnyAsync())
            return;

        // Create sample mentors
        var mentors = new List<User>
        {
            new User
            {
                Email = "mentor1@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                Name = "김앞단",
                Role = UserRole.Mentor,
                Bio = "Frontend mentor with 5+ years experience",
                Skills = new List<string> { "React", "Vue", "JavaScript", "TypeScript" }
            },
            new User
            {
                Email = "mentor2@example.com", 
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                Name = "이뒷단",
                Role = UserRole.Mentor,
                Bio = "Backend mentor specializing in .NET and Node.js",
                Skills = new List<string> { "C#", "ASP.NET Core", "Node.js", "SQL Server" }
            },
            new User
            {
                Email = "mentor3@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                Name = "박풀스택",
                Role = UserRole.Mentor,
                Bio = "Full-stack developer and mentor",
                Skills = new List<string> { "React", "Spring Boot", "Python", "AWS" }
            }
        };

        // Create sample mentees
        var mentees = new List<User>
        {
            new User
            {
                Email = "mentee1@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                Name = "최신입",
                Role = UserRole.Mentee,
                Bio = "Computer science student looking for mentorship"
            },
            new User
            {
                Email = "mentee2@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                Name = "정학습",
                Role = UserRole.Mentee,
                Bio = "Career changer interested in web development"
            },
            new User
            {
                Email = "mentee3@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                Name = "한열정",
                Role = UserRole.Mentee,
                Bio = "Junior developer seeking guidance"
            }
        };

        // Add users to context
        context.Users.AddRange(mentors);
        context.Users.AddRange(mentees);

        await context.SaveChangesAsync();
    }
}
