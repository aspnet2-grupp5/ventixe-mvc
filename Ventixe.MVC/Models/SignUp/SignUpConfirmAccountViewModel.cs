using System.ComponentModel.DataAnnotations;

namespace Ventixe.MVC.Models.SignUp;

public class SignUpConfirmAccountViewModel
{
    [Required]
    public string Email { get; set; } = null!;

    [Required]
    public string VerificationCode { get; set; } = null!;
}
