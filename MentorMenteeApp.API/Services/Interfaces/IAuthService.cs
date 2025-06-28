using MentorMenteeApp.API.DTOs.Auth;
using MentorMenteeApp.API.Models.Entities;

namespace MentorMenteeApp.API.Services.Interfaces;

public interface IAuthService
{
    Task<User?> RegisterAsync(SignupRequest request);
    Task<LoginResponse?> LoginAsync(LoginRequest request);
    Task<User?> GetUserByIdAsync(int userId);
}
