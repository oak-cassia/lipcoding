using System.ComponentModel.DataAnnotations;

namespace MentorMenteeApp.Web.Models;

public class LoginRequest
{
    [Required(ErrorMessage = "이메일을 입력해주세요.")]
    [EmailAddress(ErrorMessage = "올바른 이메일 형식을 입력해주세요.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "비밀번호를 입력해주세요.")]
    public string Password { get; set; } = string.Empty;
}
