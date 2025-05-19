using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Linq;
using Ventixe.MVC.Models;
using Ventixe.MVC.Models.Dto;

namespace Ventixe.MVC.Controllers
{
    [Authorize] // Alla actions kräver inloggning
    public class BookingsController : Controller
    {
        private readonly HttpClient _httpClient;

        public BookingsController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ventixe.bookings");
        }

        [Authorize(Roles = "Admin")] // Endast Admin kan se alla bokningar
        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("/bookings/admin-bookings");
            var bookings = await response.Content.ReadFromJsonAsync<List<BookingsModel>>();
            return View(bookings);
        }

        [Authorize(Roles = "Member")] // Endast Medlemmar ser sina egna bokningar
        public async Task<IActionResult> UserBookings()
        {
            var response = await _httpClient.GetAsync("/bookings/user-bookings");
            var bookings = await response.Content.ReadFromJsonAsync<List<BookingsModel>>();
            return View("Index", bookings);
        }

        [Authorize(Roles = "Admin,Member")] // Både Admin och Member kan skapa bokning
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Member")] // Både Admin och Member kan skapa bokning
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateBookingDto dto)
        {
            if (!ModelState.IsValid) return View(dto);
            var response = await _httpClient.PostAsJsonAsync("/bookings", dto);
            if (response.IsSuccessStatusCode) return RedirectToAction(nameof(Index));
            ModelState.AddModelError(string.Empty, "Fel vid skapande av bokning");
            return View(dto);
        }

        [Authorize(Roles = "Admin")] // Endast Admin kan redigera
        public async Task<IActionResult> Edit(string id)
        {
            var response = await _httpClient.GetAsync($"/bookings/{id}");
            if (!response.IsSuccessStatusCode) return NotFound();
            var booking = await response.Content.ReadFromJsonAsync<BookingsModel>();
            return View(booking);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")] // Endast Admin kan redigera
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, UpdateBookingDto dto)
        {
            if (!ModelState.IsValid) return View(dto);
            var response = await _httpClient.PutAsJsonAsync($"/bookings/{id}", dto);
            if (response.IsSuccessStatusCode) return RedirectToAction(nameof(Index));
            ModelState.AddModelError(string.Empty, "Fel vid uppdatering av bokning");
            return View(dto);
        }

        [Authorize(Roles = "Admin")] // Endast Admin kan ta bort
        public async Task<IActionResult> Delete(string id)
        {
            var response = await _httpClient.GetAsync($"/bookings/{id}");
            if (!response.IsSuccessStatusCode) return NotFound();
            var booking = await response.Content.ReadFromJsonAsync<BookingsModel>();
            return View(booking);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")] // Endast Admin kan ta bort
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var response = await _httpClient.DeleteAsync($"/bookings/{id}");
            if (response.IsSuccessStatusCode) return RedirectToAction(nameof(Index));
            ModelState.AddModelError(string.Empty, "Fel vid borttagning av bokning");
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Member")] // Endast Medlemmar kan avboka
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(string id)
        {
            var response = await _httpClient.PatchAsync($"/bookings/{id}/cancel", null);
            if (response.IsSuccessStatusCode) return RedirectToAction(nameof(UserBookings));
            ModelState.AddModelError(string.Empty, "Fel vid avbokning");
            return RedirectToAction(nameof(UserBookings));
        }
    }
}
