using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Ventixe.MVC.Models
{
    public class CreateEventViewModel
    {
        public string? EventId { get; set; }

        [Required(ErrorMessage = "Title is mandatory")]
        [Display(Name = "Title")]
        public string EventTitle { get; set; }=null!;

        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Display(Name = "Image-URL")]
        public string? Image { get; set; }

        [Required(ErrorMessage = "Date is mandatory")]
        [DataType(DataType.Date)]
        [Display(Name = "Date")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Price is mandatory")]
        [Range(0, 99999, ErrorMessage = "Price must be positive")]
        [Display(Name = "Price (kr)")]
        public double Price { get; set; }

        [Required(ErrorMessage = "Quantity is mandatory")]
        [Range(1, 10000, ErrorMessage = "Quantity must be at least 1")]
        [Display(Name = "Number of tickets")]
        public int Quantity { get; set; }

        public int SoldQuantity { get; set; } = 0;

        [Display(Name = "Category")]
        public string CategoryName { get; set; } = null!;
        public string? CategoryId { get; set; }

        [Display(Name = "Location")]
        public string Location { get; set; } = null!;
        public string? LocationId { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; } = null!;
        public string? StatusId { get; set; } 

        // Dropdown-listor
        public List<SelectListItem> Categories { get; set; } = new();
        public List<SelectListItem> Locations { get; set; } = new();
        public List<SelectListItem> Statuses { get; set; } = new();
    }
}
