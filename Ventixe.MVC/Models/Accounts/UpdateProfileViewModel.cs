using System.ComponentModel.DataAnnotations;

namespace Ventixe.MVC.Models.Accounts;

public class UpdateProfileViewModel
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    [Display(Name = "First Name", Prompt = "Enter your first name")]
    public string FirstName { get; set; } = null!;

    [Required]
    [StringLength(100, MinimumLength = 2)]
    [Display(Name = "Last Name", Prompt = "Enter your last name")]
    public string LastName { get; set; } = null!;

    [Required]
    [Phone]
    [Display(Name = "Phone", Prompt = "Enter your phone number")]
    public string Phone { get; set; } = null!;

    [Required]
    [StringLength(200)]
    [Display(Name = "Street Name", Prompt = "Enter street name")]
    public string StreetName { get; set; } = null!;

    [Required]
    [RegularExpression(@"^\d{5}$")]
    [Display(Name = "Postal Code", Prompt = "Enter postal code")]
    public string PostalCode { get; set; } = null!;

    [Required]
    [StringLength(100)]
    [Display(Name = "City", Prompt = "Enter city")]
    public string City { get; set; } = null!;
}
