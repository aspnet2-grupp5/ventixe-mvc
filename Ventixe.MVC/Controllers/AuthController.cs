using Authentication.Services;
using Microsoft.AspNetCore.Mvc;
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

        var existsResponse = await _http.PostAsJsonAsync("", model);
        if (existsResponse.IsSuccessStatusCode)
        {
            ViewBag.ErrorMessage = "An account already exists.";
            return View(nameof(SignUpEmail), model);
        }

        var requestVerificationCodeResponse = await _http.PostAsJsonAsync("", model);
        if (!existsResponse.IsSuccessStatusCode)
        {
            ViewBag.ErrorMessage = "Unable to send verification code.";
            return View(nameof(SignUpEmail), model);
        }

        return RedirectToAction(nameof(SignUpConfirmAccount), new SignUpConfirmAccountViewModel { Email = model.Email });
    }

    [HttpGet("auth/confirm-account")]
    public IActionResult SignUpConfirmAccount(SignUpConfirmAccountViewModel model)
    {
        return View(model);
    }

    [HttpPost("auth/confirm-account")]
    public async Task<IActionResult> HandleSignUpConfirmAccount(SignUpConfirmAccountViewModel model)
    {
        if (string.IsNullOrWhiteSpace(model.Email))
            return RedirectToAction(nameof(SignUpEmail));

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

        return RedirectToAction(nameof(SignUpPassword), new SignUpPasswordViewModel { Email = model.Email });
    }

    [HttpGet("auth/password")]
    public IActionResult SignUpPassword(SignUpPasswordViewModel model)
    {
        if (string.IsNullOrWhiteSpace(model.Email))
            return RedirectToAction(nameof(SignUpEmail));

        return View(model);
    }

    [HttpPost("auth/password")]
    public async Task<IActionResult> HandleSignUpPassword(SignUpPasswordViewModel model)
    {
        if (string.IsNullOrWhiteSpace(model.Email))
            return RedirectToAction(nameof(SignUpEmail));

        if (!ModelState.IsValid)
            return View(nameof(SignUpPassword), model);

        var response = await _http.PostAsJsonAsync("", model);
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
