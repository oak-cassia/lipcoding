using Microsoft.EntityFrameworkCore;
using MentorMenteeApp.API.Data;
using MentorMenteeApp.API.DTOs.User;
using MentorMenteeApp.API.Enums;
using MentorMenteeApp.API.Services.Interfaces;

namespace MentorMenteeApp.API.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _context;

    public UserService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<UserProfileResponse?> GetUserProfileAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return null;

        return new UserProfileResponse
        {
            Id = user.Id,
            Email = user.Email,
            Role = user.Role.ToString().ToLowerInvariant(),
            Profile = new ProfileData
            {
                Name = user.Name,
                Bio = user.Bio,
                ImageUrl = $"/images/{user.Role.ToString().ToLowerInvariant()}/{user.Id}",
                Skills = user.Role == UserRole.Mentor ? user.Skills : null
            }
        };
    }

    public async Task<UserProfileResponse?> UpdateProfileAsync(UpdateProfileRequest request)
    {
        var user = await _context.Users.FindAsync(request.Id);
        if (user == null) return null;

        // Update basic profile info
        user.Name = request.Name;
        user.Bio = request.Bio;

        // Update skills for mentors
        if (user.Role == UserRole.Mentor && request.Skills != null)
        {
            user.Skills = request.Skills;
        }

        // Update profile image if provided
        if (!string.IsNullOrEmpty(request.Image))
        {
            try
            {
                var imageBytes = Convert.FromBase64String(request.Image);
                
                // Validate image size (max 1MB)
                if (imageBytes.Length > 1024 * 1024)
                    return null;

                user.ProfileImage = imageBytes;
            }
            catch (FormatException)
            {
                // Invalid base64 string
                return null;
            }
        }

        await _context.SaveChangesAsync();

        return await GetUserProfileAsync(user.Id);
    }

    public async Task<byte[]?> GetProfileImageAsync(string role, int id)
    {
        var user = await _context.Users.FindAsync(id);
        
        if (user == null || !user.Role.ToString().Equals(role, StringComparison.OrdinalIgnoreCase))
            return null;

        return user.ProfileImage;
    }
}
