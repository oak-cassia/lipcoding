using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MentorMenteeApp.API.DTOs.MatchRequest;
using MentorMenteeApp.API.Services.Interfaces;

namespace MentorMenteeApp.API.Controllers;

[ApiController]
[Route("api")]
[Authorize]
public class MatchRequestsController : ControllerBase
{
    private readonly IMatchRequestService _matchRequestService;

    public MatchRequestsController(IMatchRequestService matchRequestService)
    {
        _matchRequestService = matchRequestService;
    }

    /// <summary>
    /// 매칭 요청 생성 (멘티 전용)
    /// </summary>
    [HttpPost("match-requests")]
    public async Task<IActionResult> CreateMatchRequest([FromBody] CreateMatchRequestDto request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (userRole != "mentee")
                return Forbid();

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized();

            // Ensure mentee can only create requests for themselves
            if (request.MenteeId != userId)
                return Forbid();

            var result = await _matchRequestService.CreateMatchRequestAsync(request);
            if (result == null)
                return BadRequest("Cannot create match request. Mentor not found or you already have a pending/accepted request.");

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// 받은 요청 목록 (멘토 전용)
    /// </summary>
    [HttpGet("match-requests/incoming")]
    public async Task<IActionResult> GetIncomingRequests()
    {
        try
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (userRole != "mentor")
                return Forbid();

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized();

            var requests = await _matchRequestService.GetIncomingRequestsAsync(userId);
            return Ok(requests);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// 보낸 요청 목록 (멘티 전용)
    /// </summary>
    [HttpGet("match-requests/outgoing")]
    public async Task<IActionResult> GetOutgoingRequests()
    {
        try
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (userRole != "mentee")
                return Forbid();

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized();

            var requests = await _matchRequestService.GetOutgoingRequestsAsync(userId);
            return Ok(requests);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// 요청 수락 (멘토 전용)
    /// </summary>
    [HttpPut("match-requests/{id}/accept")]
    public async Task<IActionResult> AcceptRequest(int id)
    {
        try
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (userRole != "mentor")
                return Forbid();

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized();

            var result = await _matchRequestService.AcceptRequestAsync(id, userId);
            if (result == null)
                return NotFound("Match request not found or cannot be accepted");

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// 요청 거절 (멘토 전용)
    /// </summary>
    [HttpPut("match-requests/{id}/reject")]
    public async Task<IActionResult> RejectRequest(int id)
    {
        try
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (userRole != "mentor")
                return Forbid();

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized();

            var result = await _matchRequestService.RejectRequestAsync(id, userId);
            if (result == null)
                return NotFound("Match request not found or cannot be rejected");

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// 요청 취소 (멘티 전용)
    /// </summary>
    [HttpDelete("match-requests/{id}")]
    public async Task<IActionResult> CancelRequest(int id)
    {
        try
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (userRole != "mentee")
                return Forbid();

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized();

            var result = await _matchRequestService.CancelRequestAsync(id, userId);
            if (result == null)
                return NotFound("Match request not found");

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error");
        }
    }
}
