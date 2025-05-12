using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ventixe.MVC.Models;
using Ventixe.MVC.Models.Dto;

namespace Ventixe.MVC.Controllers
{
    [Authorize]
    public class BookingsController : Controller
    {
        private readonly HttpClient _httpClient;

        public BookingsController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ventixe.bookings");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(BookingFilterDto filter)
        {
            string queryString = $"?Search={filter.Search}&" +
                                  $"FromDate={filter.FromDate:yyyy-MM-dd}&" +
                                  $"ToDate={filter.ToDate:yyyy-MM-dd}&" +
                                  $"Statuses={string.Join(",", filter.Statuses ?? new List<string>())}&" +
                                  $"SortBy={filter.SortBy}&" +
                                  $"SortDesc={filter.SortDesc}&" +
                                  $"Page={filter.Page}&" +
                                  $"PageSize={filter.PageSize}";

            var response = await _httpClient.GetAsync($"/bookings/admin-bookings{queryString}");
            var bookings = response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<List<BookingsModel>>()
                : new List<BookingsModel>();

            return View(bookings);
        }

        [Authorize(Roles = "Member")] // Endast Medlemmar ser sina egna bokningar
        public async Task<IActionResult> UserBookings(BookingFilterDto filter)
        {
            string queryString = $"?Search={filter.Search}&" +
                                  $"FromDate={filter.FromDate:yyyy-MM-dd}&" +
                                  $"ToDate={filter.ToDate:yyyy-MM-dd}&" +
                                  $"Statuses={string.Join(",", filter.Statuses ?? new List<string>())}&" +
                                  $"SortBy={filter.SortBy}&" +
                                  $"SortDesc={filter.SortDesc}&" +
                                  $"Page={filter.Page}&" +
                                  $"PageSize={filter.PageSize}";

            var response = await _httpClient.GetAsync($"/bookings/user-bookings{queryString}");
            var bookings = response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<List<BookingsModel>>()
                : new List<BookingsModel>();

            return View("Index", bookings);
        }

        [Authorize(Roles = "Admin,Member")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Member")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateBookingDto dto)
        {
            if (!ModelState.IsValid) return View(dto);
            var response = await _httpClient.PostAsJsonAsync("/bookings", dto);
            if (response.IsSuccessStatusCode) return RedirectToAction(nameof(Index));
            ModelState.AddModelError(string.Empty, "Fel vid skapande av bokning");
            return View(dto);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(string id)
        {
            var response = await _httpClient.GetAsync($"/bookings/{id}");
            if (!response.IsSuccessStatusCode) return NotFound();
            var booking = await response.Content.ReadFromJsonAsync<BookingsModel>();
            return View(booking);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(string id, UpdateBookingDto dto)
        {
            if (!ModelState.IsValid) return View(dto);
            var response = await _httpClient.PutAsJsonAsync($"/bookings/{id}", dto);
            if (response.IsSuccessStatusCode) return RedirectToAction(nameof(Index));
            ModelState.AddModelError(string.Empty, "Fel vid uppdatering av bokning");
            return View(dto);
        }
    }
}
