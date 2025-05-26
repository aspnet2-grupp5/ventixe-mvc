var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient("ventixe.bookings", client =>
{
    client.BaseAddress = new Uri("https://localhost:7199/api");
});

builder.Services.AddControllersWithViews();

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
