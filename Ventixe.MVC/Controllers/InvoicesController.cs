using System.Net.Http;
using System.Text;
using Microsoft.AspNetCore.Mvc;
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

    public async Task<IActionResult> Index(int? id)
    {
        var response = await _httpClient.GetAsync("api/invoices");
        if (!response.IsSuccessStatusCode) return View("Error");

        var json = await response.Content.ReadAsStringAsync();
        var invoices = JsonConvert.DeserializeObject<List<InvoiceDto>>(json);

        var selectedInvoice = id.HasValue ? invoices.FirstOrDefault(i => i.Id == id.Value) : invoices.FirstOrDefault();

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

        var jsonContent = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("api/invoices", jsonContent);

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction("Index");
        }

        ModelState.AddModelError(string.Empty, "Kunde inte skapa faktura. Försök igen.");
        return View(dto);
    }
}
