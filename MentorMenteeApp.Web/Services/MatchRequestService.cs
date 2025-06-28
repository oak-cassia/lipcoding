using System.Net.Http.Headers;
using System.Net.Http.Json;
using MentorMenteeApp.Web.Models;

namespace MentorMenteeApp.Web.Services;

public class MatchRequestService : IMatchRequestService
{
    private readonly HttpClient _httpClient;
    private readonly IAuthService _authService;
    private const string API_BASE_URL = "http://localhost:8080/api";

    public MatchRequestService(HttpClient httpClient, IAuthService authService)
    {
        _httpClient = httpClient;
        _authService = authService;
    }

    public async Task<bool> CreateMatchRequestAsync(MatchRequestCreate request)
    {
        try
        {
            var token = await _authService.GetTokenAsync();
            if (string.IsNullOrEmpty(token)) return false;

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            
            var response = await _httpClient.PostAsJsonAsync($"{API_BASE_URL}/match-requests", request);
            return response.IsSuccessStatusCode;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<List<MatchRequest>> GetMyRequestsAsync()
    {
        try
        {
            var token = await _authService.GetTokenAsync();
            if (string.IsNullOrEmpty(token)) return new List<MatchRequest>();

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            
            var response = await _httpClient.GetAsync($"{API_BASE_URL}/match-requests");
            
            if (response.IsSuccessStatusCode)
            {
                var requests = await response.Content.ReadFromJsonAsync<List<MatchRequest>>();
                return requests ?? new List<MatchRequest>();
            }
            
            return new List<MatchRequest>();
        }
        catch (Exception)
        {
            return new List<MatchRequest>();
        }
    }

    public async Task<List<MatchRequest>> GetReceivedRequestsAsync()
    {
        try
        {
            var token = await _authService.GetTokenAsync();
            if (string.IsNullOrEmpty(token)) return new List<MatchRequest>();

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            
            var response = await _httpClient.GetAsync($"{API_BASE_URL}/match-requests/received");
            
            if (response.IsSuccessStatusCode)
            {
                var requests = await response.Content.ReadFromJsonAsync<List<MatchRequest>>();
                return requests ?? new List<MatchRequest>();
            }
            
            return new List<MatchRequest>();
        }
        catch (Exception)
        {
            return new List<MatchRequest>();
        }
    }

    public async Task<bool> UpdateRequestStatusAsync(int requestId, MatchRequestUpdate update)
    {
        try
        {
            var token = await _authService.GetTokenAsync();
            if (string.IsNullOrEmpty(token)) return false;

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            
            var response = await _httpClient.PutAsJsonAsync($"{API_BASE_URL}/match-requests/{requestId}", update);
            return response.IsSuccessStatusCode;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> DeleteRequestAsync(int requestId)
    {
        try
        {
            var token = await _authService.GetTokenAsync();
            if (string.IsNullOrEmpty(token)) return false;

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            
            var response = await _httpClient.DeleteAsync($"{API_BASE_URL}/match-requests/{requestId}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
