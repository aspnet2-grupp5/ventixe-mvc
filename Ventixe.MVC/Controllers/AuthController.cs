using Microsoft.AspNetCore.Mvc;
using Ventixe.Authentication.Services;
using Ventixe.MVC.Models.Authentication.SignUp;

namespace Ventixe.MVC.Controllers;

public class AuthController(IAuthService authService, HttpClient http) : Controller
{
    private readonly IAuthService _authService = authService;
    private readonly HttpClient _http = http;

    [HttpGet("auth/signup")]
    public IActionResult SignUpEmail()
    {
        return View();
    }

    [HttpPost("auth/signup")]
    public async Task<IActionResult> HandleSignUpEmail(SignUpEmailViewModel model)
    {
        if (!ModelState.IsValid)
            return View(nameof(SignUpEmail), model);

        var existsResponse = await _http.PostAsJsonAsync("https://domain.com/api/users/exists", model);
        if (existsResponse.IsSuccessStatusCode)
        {
            ViewBag.ErrorMessage = "An account already exists.";
            return View(nameof(SignUpEmail), model);
        }

        if (!await _authService.SendVerificationCodeRequestAsync(model.Email))
        {
            ViewBag.ErrorMessage = "Unable to send verification code.";
            return View(nameof(SignUpEmail), model);
        }

        TempData["Email"] = model.Email;
        return RedirectToAction(nameof(SignUpConfirmAccount));
    }

    [HttpGet("auth/confirm-account")]
    public IActionResult SignUpConfirmAccount()
    {
        if (string.IsNullOrWhiteSpace(TempData["Email"]?.ToString()))
            return RedirectToAction(nameof(SignUpEmail));

        return View();
    }

    [HttpPost("auth/confirm-account")]
    public async Task<IActionResult> HandleSignUpConfirmAccount(SignUpConfirmAccountViewModel model)
    {
        if (string.IsNullOrWhiteSpace(model.Email))
            return RedirectToAction(nameof(SignUpEmail));

        TempData["Email"] = model.Email;

        if (string.IsNullOrWhiteSpace(model.VerificationCode))
        {
            ViewBag.ErrorMessage = "Verification code is required.";
            return View(nameof(SignUpConfirmAccount), model);
        }

        var response = await _http.PostAsJsonAsync("", model);
        if (!response.IsSuccessStatusCode)
        {
            ViewBag.ErrorMessage = "Invalid or expired verification code.";
            return View(nameof(SignUpConfirmAccount), model);
        }

        return RedirectToAction(nameof(SignUpPassword));
    }

    [HttpGet("auth/password")]
    public IActionResult SignUpPassword()
    {
        if (string.IsNullOrWhiteSpace(TempData["Email"]?.ToString()))
            return RedirectToAction(nameof(SignUpEmail));

        return View();
    }

    [HttpPost("auth/password")]
    public async Task<IActionResult> HandleSignUpPassword(SignUpPasswordViewModel model)
    {
        if (string.IsNullOrWhiteSpace(model.Email))
            return RedirectToAction(nameof(SignUpEmail));

        TempData["Email"] = model.Email;

        if (!ModelState.IsValid)
            return View(nameof(SignUpPassword), model);

        var response = await _http.PostAsJsonAsync("https://domain.com/api/users", model);
        if (!response.IsSuccessStatusCode)
        {
            ViewBag.ErrorMessage = "Unable to create new account.";
            return View(nameof(SignUpPassword), model);
        }

        return RedirectToAction(nameof(Login));
    }

    [HttpGet("auth/login")]
    public IActionResult Login()
    {
        return View();
    }
}
