namespace MentorMenteeApp.API.DTOs.User;

public class MentorListResponse
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public ProfileData Profile { get; set; } = new();
}
