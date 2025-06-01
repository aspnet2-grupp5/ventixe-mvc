using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Text;
using Ventixe.MVC.Models;
using Ventixe.MVC.Models.Bookings;
using Ventixe.MVC.Models.InvoiceModels;

public class InvoicesController : Controller
{
    private readonly HttpClient _httpClient;

    public InvoicesController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("InvoiceApi");
    }


    private async Task<List<SelectListItem>> GetBookingsFromBookingApiAsync()
    {
        var bookingList = new List<SelectListItem>();

        var response = await _httpClient.GetAsync("https://webapi-84578174132.europe-north2.run.app/api/bookings");
        if (!response.IsSuccessStatusCode)
            return bookingList;

        var json = await response.Content.ReadAsStringAsync();

        var bookings = JsonConvert.DeserializeObject<List<BookingsModel>>(json);

        bookingList = bookings?.Select(b => new SelectListItem
        {
            Value = b.BookingId?.ToString(),
            Text = b.EventName
        }).ToList();

        return bookingList;
    }

    public async Task<IActionResult> Index(int? id, string? search)
    {
        var response = await _httpClient.GetAsync("api/invoices");
        if (!response.IsSuccessStatusCode)
            return View("Error");

        var json = await response.Content.ReadAsStringAsync();
        var invoices = JsonConvert.DeserializeObject<List<InvoiceDto>>(json) ?? new List<InvoiceDto>();

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.ToLower();
            invoices = invoices
                .Where(i =>
                    (!string.IsNullOrEmpty(i.InvoiceNumber) && i.InvoiceNumber.ToLower().Contains(search)) ||
                    (!string.IsNullOrEmpty(i.CustomerName) && i.CustomerName.ToLower().Contains(search)))
                .ToList();
        }

        var selectedInvoice = id.HasValue
            ? invoices.FirstOrDefault(i => i.Id == id.Value)
            : invoices.FirstOrDefault();

        ViewBag.SelectedInvoice = selectedInvoice;

        return View(invoices);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewBag.Bookings = await GetBookingsFromBookingApiAsync();
        return View();
    }


    [HttpPost]
    public async Task<IActionResult> Create(CreateInvoiceDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        var json = JsonConvert.SerializeObject(dto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("api/invoices", content);

        if (response.IsSuccessStatusCode)
            return RedirectToAction("Index");

        ModelState.AddModelError(string.Empty, "Kunde inte skapa faktura. Försök igen.");
        return View(dto);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        ViewBag.Bookings = await GetBookingsFromBookingApiAsync();

        var response = await _httpClient.GetAsync($"api/invoices/{id}");
        if (!response.IsSuccessStatusCode)
            return View("Error");

        var json = await response.Content.ReadAsStringAsync();
        var invoice = JsonConvert.DeserializeObject<UpdateInvoiceDto>(json);

        return View(invoice);
    }


    [HttpPost]
    public async Task<IActionResult> Edit(UpdateInvoiceDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        var json = JsonConvert.SerializeObject(dto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PutAsync($"api/invoices/{dto.Id}", content);

        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError(string.Empty, "Kunde inte uppdatera faktura.");
            return View(dto);
        }

        return RedirectToAction("Index", new { id = dto.Id });
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var response = await _httpClient.DeleteAsync($"api/invoices/{id}");

        if (!response.IsSuccessStatusCode)
            TempData["Error"] = "Kunde inte ta bort fakturan.";

        return RedirectToAction("Index");
    }
}