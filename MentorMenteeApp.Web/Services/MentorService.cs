using System.Net.Http.Headers;
using System.Net.Http.Json;
using MentorMenteeApp.Web.Models;

namespace MentorMenteeApp.Web.Services;

public class MentorService : IMentorService
{
    private readonly HttpClient _httpClient;
    private readonly IAuthService _authService;
    private const string API_BASE_URL = "http://localhost:8080/api";

    public MentorService(HttpClient httpClient, IAuthService authService)
    {
        _httpClient = httpClient;
        _authService = authService;
    }

    public async Task<List<MentorListItem>> GetMentorsAsync(string? skillFilter = null, string? sortBy = null)
    {
        try
        {
            var token = await _authService.GetTokenAsync();
            if (string.IsNullOrEmpty(token)) return new List<MentorListItem>();

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var queryParams = new List<string>();
            if (!string.IsNullOrEmpty(skillFilter))
                queryParams.Add($"skill={Uri.EscapeDataString(skillFilter)}");
            if (!string.IsNullOrEmpty(sortBy))
                queryParams.Add($"sortBy={Uri.EscapeDataString(sortBy)}");

            var queryString = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";
            var url = $"{API_BASE_URL}/mentors{queryString}";

            var response = await _httpClient.GetAsync(url);
            
            if (response.IsSuccessStatusCode)
            {
                var mentors = await response.Content.ReadFromJsonAsync<List<MentorListItem>>();
                return mentors ?? new List<MentorListItem>();
            }
            
            return new List<MentorListItem>();
        }
        catch (Exception)
        {
            return new List<MentorListItem>();
        }
    }
}
