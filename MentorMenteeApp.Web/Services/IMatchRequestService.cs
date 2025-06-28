using MentorMenteeApp.Web.Models;

namespace MentorMenteeApp.Web.Services;

public interface IMatchRequestService
{
    Task<bool> CreateMatchRequestAsync(MatchRequestCreate request);
    Task<List<MatchRequest>> GetMyRequestsAsync();
    Task<List<MatchRequest>> GetReceivedRequestsAsync();
    Task<bool> UpdateRequestStatusAsync(int requestId, MatchRequestUpdate update);
    Task<bool> DeleteRequestAsync(int requestId);
}
