using Authentication.Services;
using Microsoft.AspNetCore.Mvc;
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
    public async Task<IActionResult> SignUpEmail(SignUpEmailViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        if (await _authService.AlreadyExistsAsync(model.Email))
        {
            ViewBag.ErrorMessage = "An account already exists.";
            return View(model);
        }

        return RedirectToAction("Verify");
    }
}
