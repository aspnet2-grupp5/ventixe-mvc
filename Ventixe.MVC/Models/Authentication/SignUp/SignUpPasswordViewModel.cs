using System.ComponentModel.DataAnnotations;

namespace Ventixe.MVC.Models.Authentication.SignUp;

public class SignUpPasswordViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    [DataType(DataType.Password)]
    [RegularExpression(@"^(?=.[a-z])(?=.[A-Z])(?=.\d)(?=.[^\da-zA-Z]).{8,}$",
        ErrorMessage = "Password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, one number, and one special character.")]
    [Display(Name = "Password", Prompt = "Enter password")]
    public string Password { get; set; } = null!;

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Confirm Password", Prompt = "Confirm password")]
    [Compare(nameof(Password), ErrorMessage = "Passwords do not match")]
    public string ConfirmedPassword { get; set; } = null!;
}
