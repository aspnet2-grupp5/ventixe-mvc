﻿using System.ComponentModel.DataAnnotations;

namespace Ventixe.MVC.Models.Accounts;

public class AccountProfileViewModel
{
    [Required]
    public string Id { get; set; } = null!;

    [Required]
    public string Email { get; set; } = null!;

    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string FirstName { get; set; } = null!;

    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string LastName { get; set; } = null!;

    [Required]
    [Phone]
    public string Phone { get; set; } = null!;

    [Required]
    [StringLength(200)]
    public string StreetName { get; set; } = null!;

    [Required]
    [RegularExpression(@"^\d{5}$")]
    public string PostalCode { get; set; } = null!;

    [Required]
    [StringLength(100)]
    public string City { get; set; } = null!;
}