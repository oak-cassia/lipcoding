namespace MentorMenteeApp.Web.Models;

public class MatchRequest
{
    public int Id { get; set; }
    public int MentorId { get; set; }
    public int MenteeId { get; set; }
    public string MentorName { get; set; } = string.Empty;
    public string MentorEmail { get; set; } = string.Empty;
    public string MentorProfileImageUrl { get; set; } = string.Empty;
    public string MenteeName { get; set; } = string.Empty;
    public string MenteeEmail { get; set; } = string.Empty;
    public string MenteeProfileImageUrl { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty; // Pending, Accepted, Rejected
    public DateTime CreatedAt { get; set; }
}
