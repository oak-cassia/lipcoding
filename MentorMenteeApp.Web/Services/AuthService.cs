using System.Net.Http.Json;
using System.Text.Json;
using Blazored.LocalStorage;
using MentorMenteeApp.Web.Models;

namespace MentorMenteeApp.Web.Services;

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;
    private const string API_BASE_URL = "http://localhost:8080/api";

    public AuthService(HttpClient httpClient, ILocalStorageService localStorage)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"{API_BASE_URL}/login", request);
            
            if (response.IsSuccessStatusCode)
            {
                var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
                
                if (loginResponse?.Token != null)
                {
                    await _localStorage.SetItemAsync("token", loginResponse.Token);
                    await _localStorage.SetItemAsync("user", loginResponse);
                }
                
                return loginResponse;
            }
            
            return null;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<bool> SignupAsync(SignupRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"{API_BASE_URL}/signup", request);
            return response.IsSuccessStatusCode;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task LogoutAsync()
    {
        await _localStorage.RemoveItemAsync("token");
        await _localStorage.RemoveItemAsync("user");
    }

    public async Task<string?> GetTokenAsync()
    {
        return await _localStorage.GetItemAsync<string>("token");
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var token = await GetTokenAsync();
        return !string.IsNullOrEmpty(token);
    }
}
