namespace MentorMenteeApp.API.DTOs.User;

public class UserProfileResponse
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public ProfileData Profile { get; set; } = new();
}

public class ProfileData
{
    public string Name { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public List<string>? Skills { get; set; } // Only for mentors
}
