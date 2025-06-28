using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MentorMenteeApp.API.DTOs.User;
using MentorMenteeApp.API.Services.Interfaces;

namespace MentorMenteeApp.API.Controllers;

[ApiController]
[Route("api")]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<ProfileController> _logger;

    public ProfileController(IUserService userService, ILogger<ProfileController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// 내 정보 조회
    /// </summary>
    [HttpGet("me")]
    public async Task<IActionResult> GetMyProfile()
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized();

            var profile = await _userService.GetUserProfileAsync(userId);
            if (profile == null)
                return NotFound();

            return Ok(profile);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// 프로필 수정
    /// </summary>
    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized();

            // Ensure user can only update their own profile
            if (request.Id != userId)
                return Forbid();

            var updatedProfile = await _userService.UpdateProfileAsync(request);
            if (updatedProfile == null)
                return BadRequest("Profile update failed");

            return Ok(updatedProfile);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// 프로필 이미지 조회
    /// </summary>
    [HttpGet("/images/{role}/{id}")]
    public async Task<IActionResult> GetProfileImage(string role, int id)
    {
        try
        {
            var imageBytes = await _userService.GetProfileImageAsync(role, id);
            
            if (imageBytes == null)
            {
                // Return default image based on role
                var defaultImageUrl = role.ToLowerInvariant() == "mentor" 
                    ? "https://placehold.co/500x500.jpg?text=MENTOR"
                    : "https://placehold.co/500x500.jpg?text=MENTEE";
                
                return Redirect(defaultImageUrl);
            }

            return File(imageBytes, "image/jpeg");
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error");
        }
    }
}
