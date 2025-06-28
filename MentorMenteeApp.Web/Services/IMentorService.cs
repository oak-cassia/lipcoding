using MentorMenteeApp.Web.Models;

namespace MentorMenteeApp.Web.Services;

public interface IMentorService
{
    Task<List<MentorListItem>> GetMentorsAsync(string? skillFilter = null, string? sortBy = null);
}
