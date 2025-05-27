using System.ComponentModel.DataAnnotations;

namespace Ventixe.MVC.Models.Authentication;

// temporär klass
public class DeleteUserViewModel
{
    [Required]
    [EmailAddress]
    [Display(Name = "Email", Prompt = "Enter user email to delete")]
    public string Email { get; set; } = string.Empty;
}