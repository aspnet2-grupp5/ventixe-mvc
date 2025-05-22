using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Ventixe.MVC.Models;
using Ventixe.MVC.Models.InvoiceModels;

public class InvoicesController : Controller
{
    private readonly HttpClient _httpClient;

    public InvoicesController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("InvoiceApi");
    }

    public async Task<IActionResult> Index(int? id)
    {
        var response = await _httpClient.GetAsync("api/invoices");
        if (!response.IsSuccessStatusCode) return View("Error");

        var json = await response.Content.ReadAsStringAsync();
        var invoices = JsonSerializer.Deserialize<List<InvoiceDto>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        var selectedInvoice = id.HasValue ? invoices?.FirstOrDefault(i => i.Id == id.Value) : invoices?.FirstOrDefault();

        ViewBag.SelectedInvoice = selectedInvoice;
        return View(invoices);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateInvoiceDto dto)
    {
        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        var json = JsonSerializer.Serialize(dto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("api/invoices", content);

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction("Index");
        }

        ModelState.AddModelError(string.Empty, "Kunde inte skapa faktura. Försök igen.");
        return View(dto);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
      
        var response = await _httpClient.GetAsync($"api/invoices/{id}");
        if (!response.IsSuccessStatusCode)
            return View("Error");

        var json = await response.Content.ReadAsStringAsync();
        var invoice = JsonSerializer.Deserialize<UpdateInvoiceDto>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return View(invoice);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(UpdateInvoiceDto dto)
    {
        Debug.WriteLine("RECEIVED DTO IN MVC:");
        Debug.WriteLine($"IssuedDate: {dto.IssuedDate}");
        Debug.WriteLine($"Amount: {dto.Amount}");
        Debug.WriteLine($"CustomerName: {dto.CustomerName}");
        if (!ModelState.IsValid)
            return View(dto);

        var json = JsonSerializer.Serialize(dto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PutAsync($"api/invoices/{dto.Id}", content);

        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError(string.Empty, "Kunde inte uppdatera faktura.");
            return View(dto);
        }

        return RedirectToAction("Index", new { id = dto.Id });
    }
}
