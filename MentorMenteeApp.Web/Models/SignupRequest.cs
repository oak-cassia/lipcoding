using System.ComponentModel.DataAnnotations;

namespace MentorMenteeApp.Web.Models;

public class SignupRequest
{
    [Required(ErrorMessage = "이름을 입력해주세요.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "이메일을 입력해주세요.")]
    [EmailAddress(ErrorMessage = "올바른 이메일 형식을 입력해주세요.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "비밀번호를 입력해주세요.")]
    [MinLength(6, ErrorMessage = "비밀번호는 최소 6자 이상이어야 합니다.")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "역할을 선택해주세요.")]
    public string Role { get; set; } = string.Empty;
}
