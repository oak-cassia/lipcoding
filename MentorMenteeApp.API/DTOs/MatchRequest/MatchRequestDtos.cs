using System.ComponentModel.DataAnnotations;

namespace MentorMenteeApp.API.DTOs.MatchRequest;

public class CreateMatchRequestDto
{
    [Required]
    public int MentorId { get; set; }

    [Required]  
    public int MenteeId { get; set; }

    public string Message { get; set; } = string.Empty;
}

public class MatchRequestResponse
{
    public int Id { get; set; }
    public int MentorId { get; set; }
    public int MenteeId { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

public class OutgoingMatchRequestResponse
{
    public int Id { get; set; }
    public int MentorId { get; set; }
    public int MenteeId { get; set; }
    public string Status { get; set; } = string.Empty;
}
