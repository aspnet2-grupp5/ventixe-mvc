﻿using Microsoft.AspNetCore.Mvc;
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

        var exists = await _authService.AlreadyExistsAsync(model.Email);
        if (exists.Content)
        {
            ViewBag.ErrorMessage = "An account already exists.";
            return View(nameof(SignUpEmail), model);
        }

        var result = await _authService.SendVerificationCodeRequestAsync(model.Email);
        if (!result.Content)
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

        var result = await _authService.RequestCodeValidationAsync(model.Email, model.VerificationCode);
        if (!result.Content)
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

        if (!ModelState.IsValid)
            return View(nameof(SignUpPassword), model);

        var result = await _authService.CreateUserAsync(model.Email, model.Password);
        if (!result.Succeeded)
        {
            ViewBag.ErrorMessage = "Unable to create new user.";
            return View(nameof(SignUpPassword), model);
        }

        TempData["UserId"] = result.Content!;
        TempData["Email"] = model.Email;
        return RedirectToAction(nameof(AccountsController.CreateAccount), "Accounts");
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
            var result = await _authService.LoginAsync(model.Email, model.Password, model.IsPersistent);
            if (result.Content)
                return LocalRedirect(returnUrl);
        }

        ViewBag.ErrorMessage = "Invalid email or password";
        return View(model);
    }

    public async Task<IActionResult> LogOut()
    {
        await _authService.LogOutAsync();
        return LocalRedirect("/");
    }

    #region Delete User
    // Endast för att enkelt och säkert ta bort en IdentityUser från databasen
    [HttpGet("auth/delete-user")]
    public IActionResult DeleteUser()
    {
        return View(new DeleteUserViewModel());
    }

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
    #endregion
}
