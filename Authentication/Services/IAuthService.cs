﻿
using Microsoft.AspNetCore.Identity;

namespace Ventixe.Authentication.Services
{
    public interface IAuthService
    {
        Task<bool> AlreadyExistsAsync(string email);
        Task<bool> CreateAccountAsync(string email, string password);
        Task<IdentityResult> DeleteUserAsync(string email);
        Task<bool> LoginAsync(string email, string password);
        Task<bool> RequestCodeValidationAsync(string email, string code);
        Task<bool> SendVerificationCodeRequestAsync(string email);
    }
}