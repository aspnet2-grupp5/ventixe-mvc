using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Ventixe.Authentication.Data.Entities;

namespace Ventixe.Authentication.Data.Contexts;

public class AuthenticationDbContext(DbContextOptions<AuthenticationDbContext> options) : IdentityDbContext<AppUserEntity>(options)
{
}
