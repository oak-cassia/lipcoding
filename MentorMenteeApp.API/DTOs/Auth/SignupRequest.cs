using System.ComponentModel.DataAnnotations;

namespace MentorMenteeApp.API.DTOs.Auth;

public class SignupRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;

    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Role { get; set; } = string.Empty; // "mentor" or "mentee"
}
