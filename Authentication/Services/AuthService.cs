using Microsoft.AspNetCore.Identity;
using System.Net.Http.Json;

namespace Authentication.Services;

public class AuthService(SignInManager<IdentityUser> signInManager, HttpClient httpClient) : IAuthService
{
    private readonly SignInManager<IdentityUser> _signInManager = signInManager;
    private readonly HttpClient _httpClient = httpClient;

    public async Task<bool> AlreadyExistsAsync(string email)
    {
        var result = await _httpClient.PostAsJsonAsync("", new { email });
        return true;
    }

    public async Task<bool> RequestVerificationCodeAsync(string email)
    {
        var result = await _httpClient.PostAsJsonAsync("", new { email });
        return true;
    }

    public async Task<bool> SignUpAsync(string email, string password)
    {
        var result = await _httpClient.PostAsJsonAsync("", new { email, password });
        return true;
    }
}
