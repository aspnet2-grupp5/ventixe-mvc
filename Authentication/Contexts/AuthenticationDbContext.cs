using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Ventixe.Authentication.Contexts;

public class AuthenticationDbContext(DbContextOptions options) : IdentityDbContext(options)
{
}
