using Authentication.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Ventixe.MVC.Models.SignUp;

namespace Ventixe.MVC.Controllers;

public class AuthController(IAuthService authService) : Controller
{
    private readonly IAuthService _authService = authService;

    [HttpGet("signup")]
    public IActionResult SignUpEmail()
    {
        return View();
    }

    [HttpPost("signup")]
    public async Task<IActionResult> HandleSignUpEmail(SignUpEmailViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        if (await _authService.AlreadyExistsAsync(model.Email))
        {
            ViewBag.ErrorMessage = "An account already exists.";
            return View(model);
        }

        if (!await _authService.RequestVerificationCodeAsync(model.Email))
        {
            ViewBag.ErrorMessage = "Unable to send verification code.";
            return View(model);
        }

        return RedirectToAction(nameof(SignUpConfirmAccount), new SignUpConfirmAccountViewModel { Email = model.Email });
    }

    [HttpGet("confirm-account")]
    public IActionResult SignUpConfirmAccount(SignUpConfirmAccountViewModel model)
    {
        return View(model);
    }

    [HttpPost("confirm-account")]
    public async Task<IActionResult> HandleSignUpConfirmAccount(SignUpConfirmAccountViewModel model)
    {
        if (string.IsNullOrWhiteSpace(model.Email))
            return RedirectToAction(nameof(SignUpEmail));

        if (string.IsNullOrWhiteSpace(model.VerificationCode))
        {
            ViewBag.ErrorMessage = "Verification code is required.";
            return View(model);
        }

        if (!await _authService.ValidateVerificationCodeAsync(model.Email, model.VerificationCode))
        {
            ViewBag.ErrorMessage = "Invalid or expired verification code.";
            return View(model);
        }

        return View(model);
    }
}
