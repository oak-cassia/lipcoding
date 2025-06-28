using MentorMenteeApp.API.DTOs.MatchRequest;
using MentorMenteeApp.API.Models.Entities;

namespace MentorMenteeApp.API.Services.Interfaces;

public interface IMatchRequestService
{
    Task<MatchRequestResponse?> CreateMatchRequestAsync(CreateMatchRequestDto request);
    Task<List<MatchRequestResponse>> GetIncomingRequestsAsync(int mentorId);
    Task<List<OutgoingMatchRequestResponse>> GetOutgoingRequestsAsync(int menteeId);
    Task<MatchRequestResponse?> AcceptRequestAsync(int requestId, int mentorId);
    Task<MatchRequestResponse?> RejectRequestAsync(int requestId, int mentorId);
    Task<MatchRequestResponse?> CancelRequestAsync(int requestId, int menteeId);
}
