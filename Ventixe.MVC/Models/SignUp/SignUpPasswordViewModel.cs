using System.ComponentModel.DataAnnotations;

namespace Ventixe.MVC.Models.SignUp;

public class SignUpPasswordViewModel
{
    [Required]
    public string Email { get; set; } = null!;

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Password", Prompt = "Enter password")]
    public string Password { get; set; } = null!;

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Confirm Password", Prompt = "Confirm password")]
    [Compare(nameof(Password), ErrorMessage = "Passwords do not match")]
    public string ConfirmedPassword { get; set; } = null!;
}
