using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Identity;

namespace Ventixe.Authentication.Services;

public class AuthService : IAuthService
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly ServiceBusClient _client;
    private readonly ServiceBusSender _sender;

    public AuthService(SignInManager<IdentityUser> signInManager, ServiceBusClient client)
    {
        _signInManager = signInManager;
        _client = client;
        _sender = _client.CreateSender("verification-code-requested");
    }

    public async Task<bool> SendVerificationCodeRequestAsync(string email)
    {
        try
        {
            var message = new ServiceBusMessage(email);
            await _sender.SendMessageAsync(message);

            return true;
        }
        catch
        {

            return false;
        }
    }
}
