using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace Ventixe.Authentication.Services;

public class AuthService : IAuthService
{
    private readonly HttpClient _http;
    private readonly ILogger<AuthService> _logger;
    private readonly ServiceBusClient _client;
    private readonly ServiceBusSender _sender;

    public AuthService(HttpClient http, ServiceBusClient client, ILogger<AuthService> logger)
    {
        _http = http;
        _client = client;
        _sender = _client.CreateSender("verification-code-requested");
        _logger = logger;
    }

    public async Task<bool> AlreadyExistsAsync(string email)
    {
        try
        {
            var result = await _http.PostAsJsonAsync("https://domain.com/accountservice/exists", new { email });

            return result.IsSuccessStatusCode;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request failed when checking user existence for email: {Email}", email);
            return false;
        }

    }

    public async Task<bool> SendVerificationCodeRequestAsync(string email)
    {
        try
        {
            var message = new ServiceBusMessage(email);
            await _sender.SendMessageAsync(message);

            return true;
        }
        catch (ServiceBusException ex)
        {
            _logger.LogError(ex, "Service Bus error when sending verification request message for email: {Email}", email);
            return false;
        }
    }

    public async Task<bool> RequestCodeValidationAsync(string email, string code)
    {
        try
        {
            var result = await _http.PostAsJsonAsync("https://domain.com/verificationservice/validate-verification-code", new { email, code });

            return result.IsSuccessStatusCode;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request failed during code validation for email: {Email}", email);
            return false;
        }
    }

    public async Task<bool> CreateAccountAsync(string email, string password)
    {
        try
        {
            var result = await _http.PostAsJsonAsync("https://domain.com/accountservice/api/users/create", new { email, password });

            return result.IsSuccessStatusCode;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request failed when creating account for email: {Email}", email);
            return false;
        }
    }
}
