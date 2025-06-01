using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;
using Ventixe.MVC.Models.Accounts;

namespace Ventixe.MVC.Controllers;

public class AccountsController : Controller
{
    private readonly HttpClient _http;

    public AccountsController(IHttpClientFactory httpFactory)
    {
        _http = httpFactory.CreateClient("BaseUri");
    }

    [HttpGet]
    public IActionResult CreateAccount(string id, string email)
    {
        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(email))
            return BadRequest();

        var model = new CreateAccountViewModel
        {
            UserId = id,
            Email = email
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAccount(CreateAccountViewModel model)
    {
        if (!ModelState.IsValid)
            return View(nameof(CreateAccount), model);

        var response = await _http.PostAsJsonAsync("api/accounts", model);

        if (response.StatusCode == HttpStatusCode.Conflict)
        {
            ModelState.AddModelError("", "An account profile with this user already exists.");
            return View(model);
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            ModelState.AddModelError("", "Invalid data. Please correct and submit again.");
            return View(model);
        }

        response.EnsureSuccessStatusCode();

        return RedirectToAction(nameof(AuthController.Login),"Auth");
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> AccountProfileDetails(string id)
    {
        var response = await _http.GetAsync($"api/accounts/{id}");
        if (response.StatusCode == HttpStatusCode.NotFound)
            return NotFound();

        response.EnsureSuccessStatusCode();
        var profile = await response.Content
            .ReadFromJsonAsync<AccountProfileViewModel>();

        if (profile == null)
            return NotFound();

        return View(profile);
    }
}
