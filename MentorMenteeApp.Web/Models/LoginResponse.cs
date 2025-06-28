namespace MentorMenteeApp.Web.Models;

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public UserProfile User { get; set; } = new();
}
