using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Identity;

namespace Authentication.Services;

public class AuthService : IAuthService
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly ServiceBusClient _client;
    private readonly ServiceBusSender _sender;

    public AuthService(SignInManager<IdentityUser> signInManager, ServiceBusClient client)
    {
        _signInManager = signInManager;
        _client = client;
        _sender = _client.CreateSender("auth-to-verification");
    }

}
