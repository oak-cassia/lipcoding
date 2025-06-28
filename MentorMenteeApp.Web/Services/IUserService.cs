using MentorMenteeApp.Web.Models;

namespace MentorMenteeApp.Web.Services;

public interface IUserService
{
    Task<UserProfile?> GetProfileAsync();
    Task<bool> UpdateProfileAsync(UpdateProfileRequest request);
    Task<bool> UploadProfileImageAsync(byte[] imageData, string fileName);
}
