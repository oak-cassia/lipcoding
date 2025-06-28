using System.ComponentModel.DataAnnotations;

namespace MentorMenteeApp.API.DTOs.User;

public class UpdateProfileRequest
{
    [Required]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Role { get; set; } = string.Empty;

    public string Bio { get; set; } = string.Empty;

    public string? Image { get; set; } // Base64 encoded string

    public List<string>? Skills { get; set; } // Only for mentors
}
