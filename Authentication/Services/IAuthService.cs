
namespace Authentication.Services
{
    public interface IAuthService
    {
        Task<bool> AlreadyExistsAsync(string email);
        Task<bool> SignUpAsync(string email, string password);
    }
}