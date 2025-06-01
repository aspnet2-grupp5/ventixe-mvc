using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Ventixe.Authentication.Data.Contexts;
using Ventixe.Authentication.Data.Entities;
using Ventixe.Authentication.Services;
using Ventixe.MVC.Services;
using Ventixe.MVC.Protos;
using Ventixe.Authentication.Data.Seeds;

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

builder.Services.AddScoped<IEventService, GrpcEventService>();
builder.Services.AddScoped<IGrpcEventFactory, GrpcEventFactory>();

builder.Services.AddGrpcClient<EventProto.EventProtoClient>(o =>
{
    o.Address = new Uri(builder.Configuration["GRPC:EventServiceProvider"]!);
});

builder.Services.AddGrpcClient<CategoryProto.CategoryProtoClient>(o =>
{
    o.Address = new Uri(builder.Configuration["GRPC:EventServiceProvider"]!);
});

builder.Services.AddGrpcClient<LocationProto.LocationProtoClient>(o =>
{
    o.Address = new Uri(builder.Configuration["GRPC:EventServiceProvider"]!);
});

builder.Services.AddGrpcClient<StatusProto.StatusProtoClient>(o =>
{
    o.Address = new Uri(builder.Configuration["GRPC:EventServiceProvider"]!);
});

var app = builder.Build();

//await SeedRoles.SetRolesAsync(app);

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
