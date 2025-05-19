namespace Ventixe.MVC.Models.InvoiceModels
{
    using System.ComponentModel.DataAnnotations;

    public class CreateInvoiceDto
    {
        [Required]
        public int BookingId { get; set; }

        [Required]
        [StringLength(100)]
        public string? CustomerName { get; set; }

        [Required]
        [StringLength(100)]
        public string? EventName { get; set; }

        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Amount must be greater than 0.")]
        public decimal Amount { get; set; }

        [Required]
        public DateTime IssuedDate { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        [Required]
        public string? Status { get; set; }
    }

}

