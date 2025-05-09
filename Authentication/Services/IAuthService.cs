
namespace Authentication.Services
{
    public interface IAuthService
    {
        Task<bool> AlreadyExistsAsync(string email);
        Task<bool> RequestVerificationCodeAsync(string email);
        Task<bool> SignUpAsync(string email, string password);
        Task<bool> ValidateVerificationCodeAsync(string email, string code);
    }
}