using System.Security.Claims;
using MentorMenteeApp.API.Models.Entities;

namespace MentorMenteeApp.API.Services.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user);
    ClaimsPrincipal? ValidateToken(string token);
}
