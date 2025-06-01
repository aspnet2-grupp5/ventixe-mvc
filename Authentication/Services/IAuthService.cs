
using Microsoft.AspNetCore.Identity;
using Ventixe.Authentication.Models;

namespace Ventixe.Authentication.Services
{
    public interface IAuthService
    {
        Task<AuthResult<bool>> AlreadyExistsAsync(string email);
        Task<AuthResult<string>> CreateUserAsync(string email, string password, string roleName = "Member");
        Task<IdentityResult> DeleteUserAsync(string email);
        Task<AuthResult<bool>> LoginAsync(string email, string password, bool isPersistent);
        Task<AuthResult> LogOutAsync();
        Task<AuthResult<bool>> RequestCodeValidationAsync(string email, string code);
        Task<AuthResult<bool>> SendVerificationCodeRequestAsync(string email);
    }
}