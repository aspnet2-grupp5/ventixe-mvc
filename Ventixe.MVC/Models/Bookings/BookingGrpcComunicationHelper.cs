using Ventixe.MVC.Models.Bookings.Dto;

namespace Ventixe.MVC.Models.Bookings;

public class BookingGrpcComunicationHelper
{
    private readonly HttpClient _httpClient;

    public BookingGrpcComunicationHelper(IConfiguration config)
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri($"{config["BookingAPI:BaseUri"]}/api")
        };
    }

    public async Task<bool> CreateBookingAsync(CreateBookingDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("bookings", dto);
        return response.IsSuccessStatusCode;
    }
}
