
namespace Ventixe.Authentication.Services
{
    public interface IAuthService
    {
        Task<bool> SendVerificationCodeRequestAsync(string email);
    }
}