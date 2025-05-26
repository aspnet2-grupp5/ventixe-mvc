using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Ventixe.Authentication.Data.Contexts;
using Ventixe.Authentication.Data.Entities;
using Ventixe.Authentication.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

builder.Services.AddHttpClient();
builder.Services.AddScoped(x => new ServiceBusClient(builder.Configuration.GetConnectionString("ServiceBusConnection")));
builder.Services.AddDbContext<AuthenticationDbContext>(x => x.UseSqlServer(builder.Configuration.GetConnectionString("AuthSqlConnection")));
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddIdentity<AppUserEntity, IdentityRole>(x =>
{
    x.Password.RequiredLength = 8;
    x.User.RequireUniqueEmail = true;
    //x.SignIn.RequireConfirmedAccount = true;
})
    .AddEntityFrameworkStores<AuthenticationDbContext>()
    .AddDefaultTokenProviders();

var app = builder.Build();
app.UseHsts();
app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
