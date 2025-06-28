using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using MentorMenteeApp.API.Enums;

namespace MentorMenteeApp.API.Models.Entities;

public class User
{
    [Key]
    public int Id { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public UserRole Role { get; set; }

    public string Bio { get; set; } = string.Empty;

    public byte[]? ProfileImage { get; set; }

    public string? SkillsJson { get; set; } // Store skills as JSON string

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ICollection<MatchRequest> SentRequests { get; set; } = new List<MatchRequest>();
    public ICollection<MatchRequest> ReceivedRequests { get; set; } = new List<MatchRequest>();

    // Helper property to work with skills (not mapped to database)
    [NotMapped]
    public List<string> Skills
    {
        get => string.IsNullOrEmpty(SkillsJson) 
            ? new List<string>() 
            : JsonSerializer.Deserialize<List<string>>(SkillsJson) ?? new List<string>();
        set => SkillsJson = JsonSerializer.Serialize(value);
    }
}
