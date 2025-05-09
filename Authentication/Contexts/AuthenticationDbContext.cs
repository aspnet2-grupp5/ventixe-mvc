using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Authentication.Contexts;

public class AuthenticationDbContext(DbContextOptions options) : IdentityDbContext(options)
{
}
