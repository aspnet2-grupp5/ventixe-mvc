using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ventixe.MVC.Models.Bookings;
using Ventixe.MVC.Models.Bookings.Dto;

namespace Ventixe.MVC.Controllers;

//[Authorize]
[Route("bookings")]
public class BookingsController : Controller
{
    private readonly HttpClient _httpClient;

    public BookingsController(IConfiguration config)
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri($"{config["BookingAPI:BaseUri"]}/api")
        };
    }

    [HttpGet("")]
    public async Task<IActionResult> Index([FromQuery] BookingFilterDto filter)
    {
        string queryString = $"?Search={filter.Search}&" +
                              $"SortBy={filter.SortBy}&" +
                              $"SortDesc={filter.SortDesc}&" +
                              $"FromDate={filter.FromDate:yyyy-MM-dd}&" +
                              $"ToDate={filter.ToDate:yyyy-MM-dd}&" +
                              $"Statuses={string.Join(",", filter.Status ?? new List<string>())}&" +
                              $"CategoryId={filter.CategoryId}&" +
                              $"Month={filter.Month}&" +
                              $"Page={filter.Page}&" +
                              $"PageSize={filter.PageSize}";

        var bookingsResponse = await _httpClient.GetAsync($"/bookings/admin-bookings{queryString}");
        var bookings = bookingsResponse.IsSuccessStatusCode
            ? await bookingsResponse.Content.ReadFromJsonAsync<List<BookingsModel>>()
            : new List<BookingsModel>();

        var statsResponse = await _httpClient.GetAsync("/bookings/stats");
        var stats = statsResponse.IsSuccessStatusCode
            ? await statsResponse.Content.ReadFromJsonAsync<BookingStatsDto>()
            : new BookingStatsDto();

        var vm = new BookingIndexViewModel
        {
            Bookings = bookings,
            Stats = stats,
            Filter = filter
        };

        return View(vm);
    }

    [HttpGet("user")]
        //[Authorize(Roles = "Member")]
    public async Task<IActionResult> UserBookings([FromQuery] BookingFilterDto filter)
    {
        string queryString = $"?Search={filter.Search}&" +
                             $"FromDate={filter.FromDate:yyyy-MM-dd}&" +
                             $"ToDate={filter.ToDate:yyyy-MM-dd}&" +
                             $"Statuses={string.Join(",", filter.Status ?? new List<string>())}&" +
                             $"SortBy={filter.SortBy}&" +
                             $"SortDesc={filter.SortDesc}&" +
                             $"Page={filter.Page}&" +
                             $"PageSize={filter.PageSize}";

        var response = await _httpClient.GetAsync($"/bookings/user-bookings{queryString}");
        var bookings = response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<List<BookingsModel>>()
            : new List<BookingsModel>();

        var vm = new BookingIndexViewModel
        {
            Bookings = bookings,
            Stats = null,
            Filter = filter
        };

        return View("Index", vm);
    }

    [HttpGet("create")]
    //[Authorize(Roles = "Admin,Member")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost("create")]
    //[Authorize(Roles = "Admin,Member")]
    public async Task<IActionResult> Create(CreateBookingDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        var response = await _httpClient.PostAsJsonAsync("/bookings", dto);

        if (response.IsSuccessStatusCode)
            return RedirectToAction(nameof(Index));

        ModelState.AddModelError(string.Empty, "Gick ej att skapa bokning");
            return View(dto);
    }

    [HttpGet("edit/{id}")]
    //[Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(string id)
    {
        var response = await _httpClient.GetAsync($"/bookings/{id}");

        if (!response.IsSuccessStatusCode)
            return NotFound();

        var booking = await response.Content.ReadFromJsonAsync<BookingsModel>();
            return View(booking);
    }

    [HttpPost("edit/{id}")]
    //[Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(string id, UpdateBookingDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        var response = await _httpClient.PutAsJsonAsync($"/bookings/{id}", dto);

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return NotFound();

        if (response.IsSuccessStatusCode)
            return RedirectToAction(nameof(Index));

        ModelState.AddModelError(string.Empty, "Gick ej att uppdatera bokningen");
        return View(dto);
    }
}



