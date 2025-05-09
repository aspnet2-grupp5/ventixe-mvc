using System.ComponentModel.DataAnnotations;

namespace Ventixe.MVC.Models.Authentication.SignUp;

public class SignUpConfirmAccountViewModel
{
    [Required]
    public string Email { get; set; } = null!;

    [Required]
    public string VerificationCode { get; set; } = null!;
}
