using Microsoft.AspNetCore.Mvc;
using Ventixe.MVC.Models.Vouchers;

namespace Ventixe.MVC.Controllers;

[Route("vouchers")]
public class VouchersController : Controller
{
    private readonly HttpClient _httpClient;

    public VouchersController(IConfiguration config)
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri($"{config["VoucherAPI:BaseUri"]}/api")
        };
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var response = await _httpClient.GetAsync("/vouchers");
        var vouchers = response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<List<VoucherModel>>()
            : new List<VoucherModel>();

        return View(vouchers);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Details(string id)
    {
        var response = await _httpClient.GetAsync($"/vouchers/{id}");
        var voucher = response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<VoucherModel>()
            : null;

        if (voucher == null)
            return NotFound();

        return View(voucher);
    }
}
