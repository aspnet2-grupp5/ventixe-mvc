using System.ComponentModel.DataAnnotations;

namespace Ventixe.MVC.Models.Authentication.SignUp;

public class SignUpConfirmAccountViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    public string VerificationCode { get; set; } = null!;
}
