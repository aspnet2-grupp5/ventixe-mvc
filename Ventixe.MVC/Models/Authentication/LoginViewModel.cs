using System.ComponentModel.DataAnnotations;

namespace Ventixe.MVC.Models.Authentication;

public class LoginViewModel
{
    [Required]
    [EmailAddress]
    [Display(Name = "Email", Prompt = "Email")]
    public string Email { get; set; } = null!;

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Password", Prompt = "Password")]
    public string Password { get; set; } = null!;

    //[Display(Name = "Remember me?")]
    //public bool IsPersistent { get; set; }
}
