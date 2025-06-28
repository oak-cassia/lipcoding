using Microsoft.AspNetCore.Mvc;
using MentorMenteeApp.API.DTOs.Auth;
using MentorMenteeApp.API.Services.Interfaces;

namespace MentorMenteeApp.API.Controllers;

[ApiController]
[Route("api")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// 회원가입
    /// </summary>
    [HttpPost("signup")]
    public async Task<IActionResult> Signup([FromBody] SignupRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _authService.RegisterAsync(request);
            if (user == null)
                return BadRequest("User already exists or invalid role");

            return StatusCode(201);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during user registration");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// 로그인
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _authService.LoginAsync(request);
            if (response == null)
                return Unauthorized("Invalid credentials");

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during user login");
            return StatusCode(500, "Internal server error");
        }
    }
}
