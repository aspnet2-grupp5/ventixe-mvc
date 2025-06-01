
using Microsoft.AspNetCore.Identity;
using Ventixe.Authentication.Models;

namespace Ventixe.Authentication.Services
{
    public interface IAuthService
    {
        Task<bool> AlreadyExistsAsync(string email);
        Task<AuthResult<string>> CreateUserAsync(string email, string password);
        Task<IdentityResult> DeleteUserAsync(string email);
        Task<bool> LoginAsync(string email, string password, bool isPersistent);
        Task LogOutAsync();
        Task<bool> RequestCodeValidationAsync(string email, string code);
        Task<bool> SendVerificationCodeRequestAsync(string email);
    }
}