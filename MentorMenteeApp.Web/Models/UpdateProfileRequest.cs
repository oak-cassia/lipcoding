using System.ComponentModel.DataAnnotations;

namespace MentorMenteeApp.Web.Models;

public class UpdateProfileRequest
{
    [Required(ErrorMessage = "이름을 입력해주세요.")]
    public string Name { get; set; } = string.Empty;

    public string Bio { get; set; } = string.Empty;
    public string Skills { get; set; } = string.Empty;
}
