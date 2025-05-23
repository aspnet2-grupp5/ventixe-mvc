using System.Diagnostics;
using System.Net.Http;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Ventixe.MVC.Models;
using Ventixe.MVC.Models.InvoiceModels;

public class InvoicesController : Controller
{
    private readonly HttpClient _httpClient;

    public InvoicesController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("InvoiceApi");
    }

    public async Task<IActionResult> Index(int? id, string? search)
    {
        var response = await _httpClient.GetAsync("api/invoices");
        if (!response.IsSuccessStatusCode)
            return View("Error");

        var json = await response.Content.ReadAsStringAsync();
        var invoices = JsonConvert.DeserializeObject<List<InvoiceDto>>(json) ?? new List<InvoiceDto>();

        // Sökfilter
        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.ToLower();
            invoices = invoices
                .Where(i =>
                    (!string.IsNullOrEmpty(i.InvoiceNumber) && i.InvoiceNumber.ToLower().Contains(search)) ||
                    (!string.IsNullOrEmpty(i.CustomerName) && i.CustomerName.ToLower().Contains(search)))
                .ToList();
        }

        // Välj markerad faktura
        var selectedInvoice = id.HasValue
            ? invoices.FirstOrDefault(i => i.Id == id.Value)
            : invoices.FirstOrDefault();

        ViewBag.SelectedInvoice = selectedInvoice;

        return View(invoices);
    }

    [HttpGet]
    public IActionResult Create()
    {
        ViewBag.Bookings = new List<SelectListItem>
        {
            new SelectListItem { Value = "1", Text = "Bröllop - Kund A" },
            new SelectListItem { Value = "2", Text = "Företagsevent - Kund B" },
            new SelectListItem { Value = "3", Text = "Sommarfest - Kund C" }
        };

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
        ViewBag.Bookings = new List<SelectListItem>
        {
            new SelectListItem { Value = "1", Text = "Bröllop - Kund A" },
            new SelectListItem { Value = "2", Text = "Företagsevent - Kund B" },
            new SelectListItem { Value = "3", Text = "Sommarfest - Kund C" }
        };

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
