using System.ComponentModel.DataAnnotations;

namespace Ventixe.MVC.Models.Authentication;

public class LoginViewModel
{
    [Required]
    [Display(Name = "Email", Prompt = "Enter email")]
    public string Email { get; set; } = null!;

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Password", Prompt = "Enter password")]
    public string Password { get; set; } = null!;

    [Display(Name = "Remember me?")]
    public bool IsPersistent { get; set; }
}
