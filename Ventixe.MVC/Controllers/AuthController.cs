using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ventixe.Authentication.Services;
using Ventixe.MVC.Models.Authentication;
using Ventixe.MVC.Models.Authentication.SignUp;

namespace Ventixe.MVC.Controllers;

public class AuthController(IAuthService authService) : Controller
{
    private readonly IAuthService _authService = authService;

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

        if (await _authService.AlreadyExistsAsync(model.Email))
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
        //if (string.IsNullOrWhiteSpace(TempData["Email"]?.ToString()))
        //    return RedirectToAction(nameof(SignUpEmail));

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

        if (!await _authService.RequestCodeValidationAsync(model.Email, model.VerificationCode))
        {
            ViewBag.ErrorMessage = "Invalid or expired verification code.";
            return View(nameof(SignUpConfirmAccount), model);
        }

        return RedirectToAction(nameof(SignUpPassword));
    }

    [HttpGet("auth/password")]
    public IActionResult SignUpPassword()
    {
        //if (string.IsNullOrWhiteSpace(TempData["Email"]?.ToString()))
        //    return RedirectToAction(nameof(SignUpEmail));

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

        if (!await _authService.CreateAccountAsync(model.Email, model.Password))
        {
            ViewBag.ErrorMessage = "Unable to create new account.";
            return View(nameof(SignUpPassword), model);
        }

        return RedirectToAction(nameof(Login));
    }

    [HttpGet("auth/login")]
    public IActionResult Login(string returnUrl = "/")
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Home");

        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    [HttpPost("auth/login")]
    public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = "/")
    {
        ViewBag.ReturnUrl = returnUrl;

        if (ModelState.IsValid)
        {
            if (await _authService.LoginAsync(model.Email, model.Password, model.IsPersistent))
                return LocalRedirect(returnUrl);
        }

        ViewBag.ErrorMessage = "Invalid email or password";
        return View(model);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> LogOut()
    {
        await _authService.LogOutAsync();
        return RedirectToAction(nameof(Login));
    }


    // temporär http-request
    [HttpGet("auth/delete-user")]
    public IActionResult DeleteUser()
    {
        return View(new DeleteUserViewModel());
    }

    // temporär http-request
    [HttpPost("auth/delete-user")]
    public async Task<IActionResult> DeleteUser(DeleteUserViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _authService.DeleteUserAsync(model.Email);
        if (result.Succeeded)
            return RedirectToAction("Index", "Home");

        foreach (var err in result.Errors)
            ModelState.AddModelError(string.Empty, err.Description);
        return View(model);
    }

}
