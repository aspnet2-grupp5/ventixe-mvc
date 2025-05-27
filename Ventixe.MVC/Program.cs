using Ventixe.MVC.Protos;
using Ventixe.MVC.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient("ventixe.bookings", client =>
{
    client.BaseAddress = new Uri("https://localhost:7199/api");
});

builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IEventService,GrpcEventService>();
builder.Services.AddScoped<IGrpcEventFactory, GrpcEventFactory>();


//builder.Services.AddAuthentication("MyCookieAuth")
//    .AddCookie("MyCookieAuth", options =>
//    {
//        options.LoginPath = "/Account/Login";
//        options.AccessDeniedPath = "/Account/AccessDenied";
//    });

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
app.UseHsts();
app.UseHttpsRedirection();
app.UseRouting();

//app.UseAuthentication();
//app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Event}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
