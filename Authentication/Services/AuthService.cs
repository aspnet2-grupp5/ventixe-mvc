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
    private readonly RoleManager<IdentityRole> _roleManager;

    public AuthService(HttpClient http, ServiceBusClient client, ILogger<AuthService> logger, UserManager<AppUserEntity> userManager, SignInManager<AppUserEntity> signInManager, IConfiguration configuration, RoleManager<IdentityRole> roleManager)
    {
        _http = http;
        _client = client;
        _sender = _client.CreateSender("verification-code-requested");
        _logger = logger;
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
        _roleManager = roleManager;
    }

    public async Task<AuthResult<bool>> AlreadyExistsAsync(string email)
    {
        try
        {
            var exists = await _userManager.Users.AnyAsync(x => x.UserName == email);

            return new AuthResult<bool>
            {
                Succeeded = true,
                Message = exists 
                    ? "User already exists." 
                    : "User does not exist.",
                Content = exists
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check if user exists for email: {Email}.", email);
            return new AuthResult<bool>
            {
                Succeeded = false,
                Message = $"Error checking user existence: {ex.Message}.",
                Content = false
            };
        }
    }

    public async Task<AuthResult<bool>> SendVerificationCodeRequestAsync(string email)
    {
        try
        {
            var message = new ServiceBusMessage(email);
            await _sender.SendMessageAsync(message);

            return new AuthResult<bool>
            {
                Succeeded = true,
                Message = "Verification code request sent.",
                Content = true
            };
        }
        catch (ServiceBusException ex)
        {
            _logger.LogError(ex, "Service Bus error when sending verification code request for email: {Email}.", email);
            return new AuthResult<bool>
            {
                Succeeded = false,
                Message = $"Service Bus error: {ex.Message}.",
                Content = false
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error when sending verification code request for email: {Email}.", email);
            return new AuthResult<bool>
            {
                Succeeded = false,
                Message = $"Unexpected error: {ex.Message}.",
                Content = false
            };
        }
    }

    public async Task<AuthResult<bool>> RequestCodeValidationAsync(string email, string code)
    {
        try
        {
            var validateCodeUrl = _configuration["VerificationServiceProvider:ValidateVerificationCode"];
            var response = await _http.PostAsJsonAsync(validateCodeUrl, new { email, code });

            if (response.IsSuccessStatusCode)
            {
                return new AuthResult<bool>
                {
                    Succeeded = true,
                    Message = "Verification code validation succeeded.",
                    Content = true
                };
            }
            else
            {
                return new AuthResult<bool>
                {
                    Succeeded = false,
                    Message = $"Verification code validation failed: {response.StatusCode}.",
                    Content = false
                };
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request failed during verification code validation for email: {Email}.", email);
            return new AuthResult<bool>
            {
                Succeeded = false,
                Message = $"HTTP error: {ex.Message}.",
                Content = false
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during verification code validation for email: {Email}.", email);
            return new AuthResult<bool>
            {
                Succeeded = false,
                Message = $"Unexpected error: {ex.Message}.",
                Content = false
            };
        }
    }

    public async Task<AuthResult<string>> CreateUserAsync(string email, string password, string roleName = "Member")
    {
        try
        {
            var user = new AppUserEntity
            {
                UserName = email,
                Email = email,
            };

            var createResult = await _userManager.CreateAsync(user, password);
            if (!createResult.Succeeded)
            {
                return new AuthResult<string>
                {
                    Succeeded = false,
                    Message = "Could not create user.",
                    Content = null
                };
            }

            var roleResult = await _userManager.AddToRoleAsync(user, roleName);
            if (!roleResult.Succeeded) 
            {
                return new AuthResult<string> 
                { 
                    Succeeded = false, 
                    Message = $"Could not add role: {roleName}.",
                    Content = null
                }; 
            }

            return new AuthResult<string> 
            { 
                Succeeded = true, 
                Message = $"User created with role: {roleName}.", 
                Content = user.Id 
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create Identity user with email: {Email}.", email);
            return new AuthResult<string> 
            { 
                Succeeded = false, 
                Message = $"Failed to create user: {ex.Message}.",
                Content = null
            };
        }
    }

    public async Task<AuthResult<bool>> LoginAsync(string email, string password, bool isPersistent)
    {
        try
        {
            var result = await _signInManager.PasswordSignInAsync(email, password, isPersistent, false);
            if (result.Succeeded)
            {
                //if (result.Succeeded)
                //{
                //    var token = await _httpClient.PostAsJsonAsync("/api/auth/token", payload);
                //}

                return new AuthResult<bool>
                {
                    Succeeded = true,
                    Message = "Login successful.",
                    Content = true
                };
            }
            else
            {
                return new AuthResult<bool>
                {
                    Succeeded = false,
                    Message = "Invalid login attempt.",
                    Content = false
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Login request failed for email: {Email}.", email);
            return new AuthResult<bool>
            {
                Succeeded = false,
                Message = $"Login error: {ex.Message}.",
                Content = false
            };
        }
    }

    public async Task<AuthResult> LogOutAsync()
    {
        try
        {
            await _signInManager.SignOutAsync();
            return new AuthResult
            {
                Succeeded = true,
                Message = "Logout successful."
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Logout request failed.");
            return new AuthResult<bool>
            {
                Succeeded = false,
                Message = $"Logout error: {ex.Message}."
            };
        }
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
