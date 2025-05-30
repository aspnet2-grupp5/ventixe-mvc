﻿using System.ComponentModel.DataAnnotations;

namespace Ventixe.MVC.Models.Authentication.SignUp;

public class SignUpEmailViewModel
{
    [Required]
    [EmailAddress]
    [Display(Name = "Email", Prompt = "Enter email")]
    public string Email { get; set; } = null!;
}
