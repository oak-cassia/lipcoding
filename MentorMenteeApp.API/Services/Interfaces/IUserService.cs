using MentorMenteeApp.API.DTOs.User;
using MentorMenteeApp.API.Models.Entities;

namespace MentorMenteeApp.API.Services.Interfaces;

public interface IUserService
{
    Task<UserProfileResponse?> GetUserProfileAsync(int userId);
    Task<UserProfileResponse?> UpdateProfileAsync(UpdateProfileRequest request);
    Task<byte[]?> GetProfileImageAsync(string role, int id);
}
