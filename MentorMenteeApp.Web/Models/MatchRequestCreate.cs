namespace MentorMenteeApp.Web.Models;

public class MatchRequestCreate
{
    public int MentorId { get; set; }
    public string Message { get; set; } = string.Empty;
}
