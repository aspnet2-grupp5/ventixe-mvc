using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Ventixe.Authentication.Data.Contexts;
using Ventixe.Authentication.Data.Entities;
using Ventixe.Authentication.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient();

builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient("InvoiceApi", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"]);
});


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

builder.Services.ConfigureApplicationCookie(x =>
{
    x.LoginPath = "/auth/login";
    x.AccessDeniedPath = "/";
    x.Cookie.HttpOnly = true;
    x.Cookie.IsEssential = true;
    x.Cookie.SameSite = SameSiteMode.Lax;
    x.ExpireTimeSpan = TimeSpan.FromDays(7);
    x.SlidingExpiration = true;
});

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
