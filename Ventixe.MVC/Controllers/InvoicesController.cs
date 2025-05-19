using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Ventixe.MVC.Models; // justera om din namespace är annorlunda

public class InvoicesController : Controller
{
    private readonly HttpClient _httpClient;

    public InvoicesController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("InvoiceApi");
    }

    public async Task<IActionResult> Index()
    {
        var response = await _httpClient.GetAsync("api/invoices");
        if (!response.IsSuccessStatusCode) return View("Error");

        var json = await response.Content.ReadAsStringAsync();
        var invoices = JsonConvert.DeserializeObject<List<InvoiceDto>>(json);

        return View(invoices);
    }
}

