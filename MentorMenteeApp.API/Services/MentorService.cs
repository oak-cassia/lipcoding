using Microsoft.EntityFrameworkCore;
using MentorMenteeApp.API.Data;
using MentorMenteeApp.API.DTOs.User;
using MentorMenteeApp.API.Enums;
using MentorMenteeApp.API.Services.Interfaces;

namespace MentorMenteeApp.API.Services;

public class MentorService : IMentorService
{
    private readonly AppDbContext _context;

    public MentorService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<UserProfileResponse>> GetMentorsAsync(string? skill = null, string? orderBy = null)
    {
        var query = _context.Users.Where(u => u.Role == UserRole.Mentor);

        // Filter by skill if provided
        if (!string.IsNullOrEmpty(skill))
        {
            query = query.Where(u => u.SkillsJson != null && u.SkillsJson.Contains(skill, StringComparison.OrdinalIgnoreCase));
        }

        // Apply ordering
        query = orderBy?.ToLowerInvariant() switch
        {
            "name" => query.OrderBy(u => u.Name),
            "skill" => query.OrderBy(u => u.SkillsJson),
            _ => query.OrderBy(u => u.Id) // Default: order by ID
        };

        var mentors = await query.ToListAsync();

        return mentors.Select(mentor => new UserProfileResponse
        {
            Id = mentor.Id,
            Email = mentor.Email,
            Role = mentor.Role.ToString().ToLowerInvariant(),
            Profile = new ProfileData
            {
                Name = mentor.Name,
                Bio = mentor.Bio,
                ImageUrl = $"/images/mentor/{mentor.Id}",
                Skills = mentor.Skills
            }
        }).ToList();
    }
}
