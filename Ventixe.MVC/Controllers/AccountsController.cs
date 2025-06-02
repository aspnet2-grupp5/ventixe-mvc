using Microsoft.AspNetCore.Mvc;
using System.Net;
using Ventixe.MVC.Models.Accounts;

namespace Ventixe.MVC.Controllers;

[Route("accounts")]
public class AccountsController : Controller
{
    private readonly HttpClient _http;

    public AccountsController(IHttpClientFactory httpFactory)
    {
        _http = httpFactory.CreateClient("AccountService");
    }


    [HttpGet("create-account")]
    public IActionResult CreateAccount()
    {
        var userId = TempData.Peek("UserId")?.ToString();
        var email = TempData.Peek("Email")?.ToString();

        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(email))
            return RedirectToAction(nameof(AuthController.Login), "Auth");


        var model = new CreateAccountViewModel
        {
            UserId = userId,
            Email = email
        };

        return View(model);
    }

    [HttpPost("create-account")]
    public async Task<IActionResult> HandleCreateAccount(CreateAccountViewModel model)
    {
        if (!ModelState.IsValid)
            return View(nameof(CreateAccount), model);

        var response = await _http.PostAsJsonAsync("api/accounts", model);

        if (response.StatusCode == HttpStatusCode.Conflict)
        {
            ModelState.AddModelError("", "An account profile with this user already exists.");
            return View(nameof(CreateAccount), model);
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            ModelState.AddModelError("", "Invalid data. Please correct and submit again.");
            return View(nameof(CreateAccount), model);
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
        var profile = await response.Content.ReadFromJsonAsync<AccountProfileViewModel>();

        if (profile == null)
            return NotFound();

        return View(profile);
    }

    [HttpGet("edit/{id}")]
    public async Task<IActionResult> UpdateProfileDetails(string id)
    {
        var response = await _http.GetAsync($"api/accounts/{id}");
        if (response.StatusCode == HttpStatusCode.NotFound)
            return NotFound();

        response.EnsureSuccessStatusCode();
        var profile = await response.Content.ReadFromJsonAsync<AccountProfileViewModel>();
        if (profile == null)
            return NotFound();

        var model = new UpdateProfileViewModel
        {
            FirstName = profile.FirstName,
            LastName = profile.LastName,
            Phone = profile.Phone,
            StreetName = profile.StreetName,
            PostalCode = profile.PostalCode,
            City = profile.City
        };

        return View(model);
    }

    [HttpPost("edit/{id}")]
    public async Task<IActionResult> HandleUpdateProfileDetails(string id, UpdateProfileViewModel model)
    {
        if (!ModelState.IsValid)
            return View(nameof(UpdateProfileDetails), model);

        var response = await _http.PutAsJsonAsync($"api/accounts/{id}", model);
        if (response.StatusCode == HttpStatusCode.NotFound)
            return NotFound();

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            ModelState.AddModelError("", "Invalid data. Please correct and submit again.");
            return View(nameof(UpdateProfileDetails), model);
        }

        response.EnsureSuccessStatusCode();

        return RedirectToAction(nameof(AccountProfileDetails), new { id });
    }

    [HttpPost("delete/{id}")]
    public async Task<IActionResult> DeleteAccount(string id)
    {
        var response = await _http.DeleteAsync($"api/accounts/{id}");
        if (response.StatusCode == HttpStatusCode.NotFound)
            return NotFound();

        response.EnsureSuccessStatusCode();

        return RedirectToAction("Index", "Home");
    }
}
