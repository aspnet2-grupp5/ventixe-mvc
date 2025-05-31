using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using Ventixe.Authentication.Data.Entities;
using Ventixe.Authentication.Models;

namespace Ventixe.Authentication.Services;

public class AuthService : IAuthService
{
    private readonly HttpClient _http;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;
    private readonly ServiceBusClient _client;
    private readonly ServiceBusSender _sender;
    private readonly UserManager<AppUserEntity> _userManager;
    private readonly SignInManager<AppUserEntity> _signInManager;

    public AuthService(HttpClient http, ServiceBusClient client, ILogger<AuthService> logger, UserManager<AppUserEntity> userManager, SignInManager<AppUserEntity> signInManager, IConfiguration configuration)
    {
        _http = http;
        _client = client;
        _sender = _client.CreateSender("verification-code-requested");
        _logger = logger;
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
    }

    public async Task<bool> AlreadyExistsAsync(string email)
    {
        var result = await _userManager.Users.AnyAsync(x => x.Email == email);

        return result;
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
            var validateCodeUrl = _configuration["VerificationServiceProvider:ValidateVerificationCode"];
            var result = await _http.PostAsJsonAsync(validateCodeUrl, new { email, code });

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
            var user = new AppUserEntity
            {
                UserName = email,
                Email = email,
            };

            var result = await _userManager.CreateAsync(user, password);
            return result.Succeeded;

            //var result = await _http.PostAsJsonAsync("https://domain.com/accountservice/api/users/create", new { email, password });

            //return result.IsSuccessStatusCode;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request failed when creating account for email: {Email}", email);
            return false;
        }
    }

    public async Task<bool> LoginAsync(string email, string password, bool isPersistent)
    {
        try
        {
            var result = await _signInManager.PasswordSignInAsync(email, password, isPersistent, false);
            return result.Succeeded;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Login request failed.");
            return false;
        }
    }

    public async Task LogOutAsync()
    {
        await _signInManager.SignOutAsync();
    }


    // Endast för att enkelt och säkert ta bort en IdentityUser från databasen
    public async Task<IdentityResult> DeleteUserAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            return IdentityResult.Failed(new IdentityError { Description = "User not found." });

        return await _userManager.DeleteAsync(user);
    }
}
