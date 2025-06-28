using MentorMenteeApp.API.DTOs.User;

namespace MentorMenteeApp.API.Services.Interfaces;

public interface IMentorService
{
    Task<List<UserProfileResponse>> GetMentorsAsync(string? skill = null, string? orderBy = null);
}
