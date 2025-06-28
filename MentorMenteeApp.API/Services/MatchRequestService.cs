using Microsoft.EntityFrameworkCore;
using MentorMenteeApp.API.Data;
using MentorMenteeApp.API.DTOs.MatchRequest;
using MentorMenteeApp.API.Enums;
using MentorMenteeApp.API.Models.Entities;
using MentorMenteeApp.API.Services.Interfaces;

namespace MentorMenteeApp.API.Services;

public class MatchRequestService : IMatchRequestService
{
    private readonly AppDbContext _context;

    public MatchRequestService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<MatchRequestResponse?> CreateMatchRequestAsync(CreateMatchRequestDto request)
    {
        // Check if mentor exists and is actually a mentor
        var mentor = await _context.Users.FindAsync(request.MentorId);
        if (mentor == null || mentor.Role != UserRole.Mentor)
            return null;

        // Check if mentee has any pending or accepted requests
        var existingRequest = await _context.MatchRequests
            .Where(mr => mr.MenteeId == request.MenteeId && 
                        (mr.Status == MatchRequestStatus.Pending || mr.Status == MatchRequestStatus.Accepted))
            .FirstOrDefaultAsync();

        if (existingRequest != null)
            return null;

        // Create new match request
        var matchRequest = new MatchRequest
        {
            MentorId = request.MentorId,
            MenteeId = request.MenteeId,
            Message = request.Message,
            Status = MatchRequestStatus.Pending
        };

        _context.MatchRequests.Add(matchRequest);
        await _context.SaveChangesAsync();

        return MapToResponse(matchRequest);
    }

    public async Task<List<MatchRequestResponse>> GetIncomingRequestsAsync(int mentorId)
    {
        var requests = await _context.MatchRequests
            .Where(mr => mr.MentorId == mentorId)
            .OrderByDescending(mr => mr.CreatedAt)
            .ToListAsync();

        return requests.Select(MapToResponse).ToList();
    }

    public async Task<List<OutgoingMatchRequestResponse>> GetOutgoingRequestsAsync(int menteeId)
    {
        var requests = await _context.MatchRequests
            .Where(mr => mr.MenteeId == menteeId)
            .OrderByDescending(mr => mr.CreatedAt)
            .ToListAsync();

        return requests.Select(MapToOutgoingResponse).ToList();
    }

    public async Task<MatchRequestResponse?> AcceptRequestAsync(int requestId, int mentorId)
    {
        var request = await _context.MatchRequests
            .FirstOrDefaultAsync(mr => mr.Id == requestId && mr.MentorId == mentorId);

        if (request == null || request.Status != MatchRequestStatus.Pending)
            return null;

        // Check if mentor already has an accepted request
        var existingAccepted = await _context.MatchRequests
            .AnyAsync(mr => mr.MentorId == mentorId && mr.Status == MatchRequestStatus.Accepted);

        if (existingAccepted)
            return null;

        request.Status = MatchRequestStatus.Accepted;
        await _context.SaveChangesAsync();

        return MapToResponse(request);
    }

    public async Task<MatchRequestResponse?> RejectRequestAsync(int requestId, int mentorId)
    {
        var request = await _context.MatchRequests
            .FirstOrDefaultAsync(mr => mr.Id == requestId && mr.MentorId == mentorId);

        if (request == null || request.Status != MatchRequestStatus.Pending)
            return null;

        request.Status = MatchRequestStatus.Rejected;
        await _context.SaveChangesAsync();

        return MapToResponse(request);
    }

    public async Task<MatchRequestResponse?> CancelRequestAsync(int requestId, int menteeId)
    {
        var request = await _context.MatchRequests
            .FirstOrDefaultAsync(mr => mr.Id == requestId && mr.MenteeId == menteeId);

        if (request == null)
            return null;

        request.Status = MatchRequestStatus.Cancelled;
        await _context.SaveChangesAsync();

        return MapToResponse(request);
    }

    private static MatchRequestResponse MapToResponse(MatchRequest request)
    {
        return new MatchRequestResponse
        {
            Id = request.Id,
            MentorId = request.MentorId,
            MenteeId = request.MenteeId,
            Message = request.Message,
            Status = request.Status.ToString().ToLowerInvariant()
        };
    }

    private static OutgoingMatchRequestResponse MapToOutgoingResponse(MatchRequest request)
    {
        return new OutgoingMatchRequestResponse
        {
            Id = request.Id,
            MentorId = request.MentorId,
            MenteeId = request.MenteeId,
            Status = request.Status.ToString().ToLowerInvariant()
        };
    }
}
