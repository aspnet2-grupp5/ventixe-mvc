using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Identity;
using System.Net.Http.Json;

namespace Authentication.Services;

public class AuthService : IAuthService
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly HttpClient _http;
    private readonly ServiceBusClient _client;
    private readonly ServiceBusSender _sender;

    public AuthService(SignInManager<IdentityUser> signInManager, HttpClient http, ServiceBusClient client)
    {
        _signInManager = signInManager;
        _http = http;
        _client = client;
        _sender = _client.CreateSender("emailservice");
    }

    public async Task<bool> AlreadyExistsAsync(string email)
    {
        try
        {
            var result = await _http.PostAsJsonAsync("", new { email });
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> RequestVerificationCodeAsync(string email)
    {
        try
        {
            var result = await _http.PostAsJsonAsync("", new { email });
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> ValidateVerificationCodeAsync(string email, string code)
    {
        try
        {
            var result = await _http.PostAsJsonAsync("", new { email, code });
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> SignUpAsync(string email, string password)
    {
        try
        {
            var result = await _http.PostAsJsonAsync("", new { email, password });
            return true;
        }
        catch
        {
            return false;
        }
    }
}
