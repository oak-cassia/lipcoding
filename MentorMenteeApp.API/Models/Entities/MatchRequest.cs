using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MentorMenteeApp.API.Enums;

namespace MentorMenteeApp.API.Models.Entities;

public class MatchRequest
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int MentorId { get; set; }

    [Required]
    public int MenteeId { get; set; }

    public string Message { get; set; } = string.Empty;

    [Required]
    public MatchRequestStatus Status { get; set; } = MatchRequestStatus.Pending;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey(nameof(MentorId))]
    public User Mentor { get; set; } = null!;

    [ForeignKey(nameof(MenteeId))]
    public User Mentee { get; set; } = null!;
}
