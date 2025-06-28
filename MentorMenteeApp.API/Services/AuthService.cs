using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using MentorMenteeApp.API.Data;
using MentorMenteeApp.API.DTOs.Auth;
using MentorMenteeApp.API.Enums;
using MentorMenteeApp.API.Models.Entities;
using MentorMenteeApp.API.Services.Interfaces;

namespace MentorMenteeApp.API.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IJwtService _jwtService;

    public AuthService(AppDbContext context, IJwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    public async Task<User?> RegisterAsync(SignupRequest request)
    {
        // Check if user already exists
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            return null;

        // Validate role
        if (!Enum.TryParse<UserRole>(request.Role, true, out var userRole))
            return null;

        // Create new user
        var user = new User
        {
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Name = request.Name,
            Role = userRole
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return user;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return null;

        var token = _jwtService.GenerateToken(user);
        return new LoginResponse { Token = token };
    }

    public async Task<User?> GetUserByIdAsync(int userId)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
    }
}
