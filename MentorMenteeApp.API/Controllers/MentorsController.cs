using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MentorMenteeApp.API.Services.Interfaces;

namespace MentorMenteeApp.API.Controllers;

[ApiController]
[Route("api")]
[Authorize]
public class MentorsController : ControllerBase
{
    private readonly IMentorService _mentorService;

    public MentorsController(IMentorService mentorService)
    {
        _mentorService = mentorService;
    }

    /// <summary>
    /// 멘토 목록 조회 (멘티 전용)
    /// </summary>
    [HttpGet("mentors")]
    public async Task<IActionResult> GetMentors([FromQuery] string? skill, [FromQuery] string? order_by)
    {
        try
        {
            // Check if user is mentee
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (userRole != "mentee")
                return Forbid();

            var mentors = await _mentorService.GetMentorsAsync(skill, order_by);
            return Ok(mentors);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error");
        }
    }
}
